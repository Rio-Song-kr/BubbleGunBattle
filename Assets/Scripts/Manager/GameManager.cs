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

    // private readonly float _defaultTime = 300f;
    [SerializeField] private float _defaultTime = 10;

    private float _timeRemaining;
    private float _prevTime;
    private bool _gameOver;
    private bool _isPaused;
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
    public Action<int[], string[]> OnGameOver;
    public Action<string, int> OnScoreAdded;
    // public Action<string, int> OnTotalScoreChanged;

    public string PlayerName { get; private set; }
    //# Fun2 적용 시 player의 ActorNumber로 이름 등록 및 점수 관리
    private Dictionary<int, string> PlayerNames = new Dictionary<int, string>();
    private Dictionary<int, int> PlayerScores = new Dictionary<int, int>();
    // public string WinnerPlayerName { get; private set; }
    // public int WinnerScore { get; private set; }

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
    }

    private void OnEnable()
    {
        InitializeTime();

        //todo 추후 게임 시작 시 3초 카운트다운 후 시작하도록 변경해야 함
        _gameOver = false;
        _isPaused = false;
    }

    private void Update()
    {
        if (IsTitle || _gameOver || _isPaused) return;

        _timeRemaining -= Time.deltaTime;

        if (_prevTime - _timeRemaining < 0.1f) return;

        if (_timeRemaining <= 0f)
        {
            _gameOver = true;
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
        PlayerNames.Add(0, PlayerName);
        PlayerScores.Add(0, 0);
    }

    private void HandleGameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        OnScoreAdded?.Invoke(PlayerName, 0);
        OnTimeChanged?.Invoke(_defaultTime);
        // WinnerPlayerName = "";
        // WinnerScore = 0;
    }

    private void HandleGameOver()
    {
        //todo Winner를 판별해야 함
        int playerActorNumber = 0;
        int maxScore = 0;

        foreach (var playerScore in PlayerScores)
        {
            if (playerScore.Value > maxScore)
            {
                playerActorNumber = playerScore.Key;
                maxScore = playerScore.Value;
            }
        }

        var sorted = PlayerScores.OrderByDescending(pair => pair.Value).ToArray();
        int[] sortedScores = sorted.Select(pair => pair.Value).ToArray();
        string[] sortedNames = sorted.Select(pair => PlayerNames[pair.Key]).ToArray();

        // WinnerPlayerName = _playerNames[playerActorNumber];
        // WinnerScore = _playerScores[playerActorNumber];

        Cursor.lockState = CursorLockMode.None;
        OnGameOver?.Invoke(sortedScores, sortedNames);
    }
}