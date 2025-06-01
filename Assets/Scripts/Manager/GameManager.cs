using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public ItemManager ItemManager { get; private set; }
    public InputManager Input { get; private set; }

    private ISettableScore _hudScore;
    private ISettableTime _hudTime;
    // private readonly float _defaultTime = 300f;
    [SerializeField] private float _defaultTime = 10;
    private float _timeRemaining;
    private float _prevTime;
    private bool _gameOver;
    private bool _isPaused;

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
    }

    private void OnEnable()
    {
        InitializeTime();

        //todo 추후 게임 시작 시 3초 카운트다운 후 시작하도록 변경해야 함
        _gameOver = false;
        _isPaused = false;
    }

    private void Start()
    {
        var hudGameObject = GameObject.FindWithTag("HUDCanvas");
        _hudTime = hudGameObject.GetComponent<ISettableTime>();
        _hudScore = hudGameObject.GetComponent<ISettableScore>();

        _hudTime.SetTime(_defaultTime);
        _hudScore.SetScore("Player", 0);
    }

    private void Update()
    {
        if (_gameOver || _isPaused) return;

        _timeRemaining -= Time.deltaTime;

        if (_prevTime - _timeRemaining < 0.1f || _timeRemaining < 0f) return;

        _hudTime.SetTime(_timeRemaining);
        _prevTime = _timeRemaining;
    }

    private void InitializeTime()
    {
        _timeRemaining = _defaultTime;
        _prevTime = _defaultTime;
    }

    public void AddScore(int score) => _hudScore.AddScore("Player", score);
}