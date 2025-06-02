using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public ItemManager ItemManager { get; private set; }
    public InputManager Input { get; private set; }
    public GlobalUIManager UI { get; private set; }
    public MySceneManager Scene { get; private set; }

    // private ISettableScore _hudScore;
    // private ISettableTime _hudTime;

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

            if (!_isTitle)
            {
                OnScoreAdded?.Invoke(PlayerName, 0);
                OnTimeChanged?.Invoke(_defaultTime);
            }
        }
    }

    public Action<float> OnTimeChanged;
    public Action<string, int> OnScoreAdded;

    public string PlayerName { get; private set; }

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

        if (_prevTime - _timeRemaining < 0.1f || _timeRemaining < 0f) return;

        OnTimeChanged?.Invoke(_timeRemaining);
        _prevTime = _timeRemaining;
    }

    private void InitializeTime()
    {
        _timeRemaining = _defaultTime;
        _prevTime = _defaultTime;
    }

    public void AddScore(int score) => OnScoreAdded?.Invoke(PlayerName, score);
    public void SetPlayerName(string name) => PlayerName = name;
}