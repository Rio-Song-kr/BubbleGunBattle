using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    private AsyncOperation _levelSceneOperation;
    private AsyncOperation _gameLogicOperation;
    private AsyncOperation _inGameUiOperation;
    private ISceneLoadable _sceneLoadUI;

    private bool _isLoading = false;
    public bool IsLoading => _isLoading;
    private string _prevSceneName = "TitleScene";
    private int _prevSceneIndex = 0;

    private void Start()
    {
        _sceneLoadUI = GameManager.Instance.UI.GetComponent<ISceneLoadable>();
    }

    //# 비동기 씬 로딩
    public void LoadSceneAsync(string sceneName, bool isTitleScene = false)
    {
        if (!_isLoading)
        {
            // if (!_prevSceneName.Equals(sceneName))
            GameManager.Instance.Audio.StopBGM();
            _prevSceneName = sceneName;
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName, isTitleScene));
        }
    }

    public void LoadSceneAsync(int sceneIndex, bool isTitleScene)
    {
        if (!_isLoading)
        {
            // if (_prevSceneIndex != sceneIndex)
            GameManager.Instance.Audio.StopBGM();
            _prevSceneIndex = sceneIndex;
            StartCoroutine(LoadSceneAsyncCoroutine(sceneIndex, isTitleScene));
        }
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, bool isTitleScene)
    {
        yield return StartCoroutine(PrepareSceneLoading());

        LoadScene(sceneName);
        if (!isTitleScene)
        {
            LoadAdditiveScene();
            yield return StartCoroutine(UpdateMultipleSceneLoadingProgress(isTitleScene));
        }
        else yield return StartCoroutine(UpdateLoadingProgress(isTitleScene));
    }

    private IEnumerator LoadSceneAsyncCoroutine(int sceneIndex, bool isTitleScene)
    {
        yield return StartCoroutine(PrepareSceneLoading());

        LoadScene(sceneIndex);
        if (!isTitleScene)
        {
            LoadAdditiveScene();
            yield return StartCoroutine(UpdateMultipleSceneLoadingProgress(isTitleScene));
        }
        else yield return StartCoroutine(UpdateLoadingProgress(isTitleScene));
    }

    private void LoadScene(string sceneName)
    {
        _levelSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        _levelSceneOperation.allowSceneActivation = false;
    }

    private void LoadScene(int sceneIndex)
    {
        _levelSceneOperation = SceneManager.LoadSceneAsync(sceneIndex);
        _levelSceneOperation.allowSceneActivation = false;
    }

    private void LoadAdditiveScene()
    {
        _gameLogicOperation = SceneManager.LoadSceneAsync("GameLogic", LoadSceneMode.Additive);
        _inGameUiOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

        _gameLogicOperation.allowSceneActivation = false;
        _inGameUiOperation.allowSceneActivation = false;
    }

    private IEnumerator PrepareSceneLoading()
    {
        _isLoading = true;
        _sceneLoadUI.InitSceneLoadingUI();

        yield return new WaitForEndOfFrame();
    }

    private IEnumerator UpdateLoadingProgress(bool isTitleScene)
    {
        while (!_levelSceneOperation.isDone)
        {
            //# 씬 활성화 및 씬이 완전히 로드될 때까지 대기
            if (_levelSceneOperation.progress >= 0.88f)
            {
                _levelSceneOperation.allowSceneActivation = true;
                yield return new WaitUntil(() => _levelSceneOperation.isDone);
            }

            //# 로딩 진행률 (0.0 ~ 0.9)
            float totalProgress = _levelSceneOperation.progress;

            if (UpdateLoadingUI(totalProgress)) break;

            yield return null;
        }

        yield return StartCoroutine(CompleteSceneLoading(isTitleScene));
    }

    private IEnumerator UpdateMultipleSceneLoadingProgress(bool isTitleScene)
    {
        while (!_levelSceneOperation.isDone || !_gameLogicOperation.isDone || _inGameUiOperation.isDone)
        {
            //# 씬 활성화 및 씬이 완전히 로드될 때까지 대기
            if (_levelSceneOperation.progress >= 0.88f)
            {
                _levelSceneOperation.allowSceneActivation = true;
                yield return new WaitUntil(() => _levelSceneOperation.isDone);
            }
            if (_gameLogicOperation.progress >= 0.88f)
            {
                _gameLogicOperation.allowSceneActivation = true;
                yield return new WaitUntil(() => _gameLogicOperation.isDone);
            }
            if (_inGameUiOperation.progress >= 0.88f)
            {
                _inGameUiOperation.allowSceneActivation = true;
                yield return new WaitUntil(() => _inGameUiOperation.isDone);
            }

            //# 로딩 진행률 (0.0 ~ 0.9)
            float totalProgress =
                (_levelSceneOperation.progress + _gameLogicOperation.progress + _inGameUiOperation.progress) / 3f;

            if (UpdateLoadingUI(totalProgress)) break;

            yield return null;
        }

        yield return StartCoroutine(CompleteSceneLoading(isTitleScene));
    }

    private bool UpdateLoadingUI(float totalProgress)
    {
        //# 0.9에서 1.0까지는 수동으로 제어
        float displayProgress = totalProgress;

        if (displayProgress >= 0.9f)
        {
            displayProgress = Mathf.Lerp(displayProgress, 1.0f, Time.deltaTime);

            if (displayProgress >= 1f)
            {
                _sceneLoadUI.UpdateLoadingUI(1.0f);
                return true;
            }
        }

        _sceneLoadUI.UpdateLoadingUI(displayProgress);
        return false;
    }

    private IEnumerator CompleteSceneLoading(bool isTitleScene)
    {
        _sceneLoadUI.CompleteSceneLoading();

        yield return new WaitForSeconds(0.3f);

        _isLoading = false;
        _levelSceneOperation = null;
        _gameLogicOperation = null;
        _inGameUiOperation = null;
        GameManager.Instance.IsTitle = isTitleScene;
        GameManager.Instance.HandleChangeScene();
    }

    public string GetActiveScene() => SceneManager.GetActiveScene().name;
}