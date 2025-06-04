using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public ItemManager ItemManager { get; private set; }
    public InputManager Input { get; private set; }
    public GlobalUIManager UI { get; private set; }
    public MySceneManager Scene { get; private set; }
    public AudioManager Audio { get; private set; }

    [SerializeField] private float _defaultTime = 10;
    public string TitleSceneName = "TitleScene";

    private float _timeRemaining;
    private float _prevTime;
    private bool _isGameOver;
    public bool IsGameOver => _isGameOver;
    private bool _isPaused;
    public bool IsPaused => _isPaused;
    private bool _isFirstChange;

    private bool _isTitle = true;

    public bool IsTitle
    {
        get => _isTitle;
        set
        {
            _isTitle = value;

            if (!_isTitle) HandleGameStart();
        }
    }

    public Action<float> OnTimeChanged;
    public Action<int[], string[], string> OnGameOver;
    public Action<string, int> OnScoreAdded;
    public Action<string, int> OnTotalScoreChanged;
    public Action OnSceneChanged;

    public string PlayerName { get; private set; }
    //# Fun2 적용 시 player의 ActorNumber로 이름 등록 및 점수 관리
    private Dictionary<int, string> PlayersName = new Dictionary<int, string>();
    private Dictionary<int, int> PlayersScore = new Dictionary<int, int>();

    public static void CreateInstance()
    {
        if (_instance == null)
        {
            var gameManagerPrefab = Resources.Load<GameManager>("GameManager");
            _instance = Instantiate(gameManagerPrefab);
            DontDestroyOnLoad(_instance);
        }
    }

    public static void ReleaseInstance()
    {
        if (_instance != null)
        {
            Destroy(_instance);
            _instance = null;
        }
    }

    private void Awake()
    {
        ItemManager = GetComponent<ItemManager>();
        Input = GetComponent<InputManager>();
        UI = GetComponent<GlobalUIManager>();
        Scene = GetComponent<MySceneManager>();
        Audio = GetComponent<AudioManager>();
    }

    private void OnEnable()
    {
        InitializeTime();

        //todo 추후 게임 시작 시 3초 카운트다운 후 시작하도록 변경해야 함
        _isGameOver = false;
        _isPaused = false;
    }

    private void Update()
    {
        if (IsTitle || _isGameOver) return;

        _timeRemaining -= Time.deltaTime;

        if (_prevTime - _timeRemaining < 0.1f) return;

        if (_timeRemaining <= 0f)
        {
            _isGameOver = true;
            HandleGameOver();
        }

        OnTimeChanged?.Invoke(_timeRemaining);
        _prevTime = _timeRemaining;
    }

    private void InitializeTime()
    {
        _timeRemaining = _defaultTime;
        _prevTime = _defaultTime;
    }

    public void AddScore(int score) => OnScoreAdded?.Invoke(PlayerName, score);

    public void SetPlayerName(string name)
    {
        PlayerName = name;

        //todo 현재는 싱글 플레이어이므로, actorNumber는 0으로 사용
        PlayersName[0] = PlayerName;
        PlayersScore[0] = 0;
    }

    private void HandleGameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        OnScoreAdded?.Invoke(PlayerName, 0);
        OnTimeChanged?.Invoke(_defaultTime);
        OnTotalScoreChanged += SetScore;
        _isGameOver = false;
        _isPaused = false;
        InitializeTime();
        Audio.Play2DSFX(AudioClipName.GameStartSound);
    }

    private void HandleGameOver()
    {
        var sorted = PlayersScore.OrderByDescending(pair => pair.Value).ToArray();
        int[] sortedScore = sorted.Select(pair => pair.Value).ToArray();
        string[] sortedName = sorted.Select(pair => PlayersName[pair.Key]).ToArray();

        Cursor.lockState = CursorLockMode.None;

        string titleText;
        if (PlayerName == sortedName[0])
        {
            titleText = "Win!";
            Audio.Play2DSFX(AudioClipName.WinSound);
        }
        else
        {
            titleText = "Lose...";
            Audio.Play2DSFX(AudioClipName.LoseSound);
        }
        OnGameOver?.Invoke(sortedScore, sortedName, titleText);
    }

    //# 게임을 다시 시작하거나 맵을 이동하면 초기화
    public void HandleChangeScene()
    {
        PlayersName = new Dictionary<int, string>();
        PlayersScore = new Dictionary<int, int>();

        //todo 현재는 싱글 플레이어이므로, actorNumber는 0으로 사용
        PlayersName[0] = PlayerName;
        PlayersScore[0] = 0;

        if (_isTitle)
        {
            OnTotalScoreChanged -= SetScore;
            Audio.PlayBGM(AudioClipName.TitleBackground);
        }
        else Audio.PlayBGM(AudioClipName.LevelBackground);
    }

    private void SetScore(string playerName, int totalScore)
    {
        int key = PlayersName.FirstOrDefault(pair => pair.Value == playerName).Key;
        PlayersScore[key] = totalScore;
    }

    public void SetPaused(bool value)
    {
        _isPaused = value;
        if (_isPaused) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }
}