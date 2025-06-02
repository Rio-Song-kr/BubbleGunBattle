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

    private void Start()
    {
        _sceneLoadUI = GameManager.Instance.UI.GetComponent<ISceneLoadable>();
    }

    //# 비동기 씬 로딩
    public void LoadSceneAsync(string sceneName)
    {
        if (!_isLoading)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }
    }

    public void LoadSceneAsync(int sceneIndex)
    {
        if (!_isLoading)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneIndex));
        }
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        yield return StartCoroutine(PrepareSceneLoading());

        _levelSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        _gameLogicOperation = SceneManager.LoadSceneAsync("GameLogic", LoadSceneMode.Additive);
        _inGameUiOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

        _levelSceneOperation.allowSceneActivation = false;
        _gameLogicOperation.allowSceneActivation = false;
        _inGameUiOperation.allowSceneActivation = false;

        yield return StartCoroutine(UpdateLoadingProgress());
    }

    private IEnumerator LoadSceneAsyncCoroutine(int sceneIndex)
    {
        yield return StartCoroutine(PrepareSceneLoading());

        _levelSceneOperation = SceneManager.LoadSceneAsync(sceneIndex);
        _gameLogicOperation = SceneManager.LoadSceneAsync("GameLogic", LoadSceneMode.Additive);
        _inGameUiOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

        _levelSceneOperation.allowSceneActivation = false;
        _gameLogicOperation.allowSceneActivation = false;
        _inGameUiOperation.allowSceneActivation = false;

        yield return StartCoroutine(UpdateLoadingProgress());
    }

    private IEnumerator PrepareSceneLoading()
    {
        _isLoading = true;
        _sceneLoadUI.InitSceneLoadingUI();

        yield return new WaitForEndOfFrame();
    }

    private IEnumerator UpdateLoadingProgress()
    {
        float timer = 0f;

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

            //# 0.9에서 1.0까지는 수동으로 제어
            float displayProgress = totalProgress;

            if (displayProgress >= 0.9f)
            {
                displayProgress = Mathf.Lerp(displayProgress, 1.0f, Time.deltaTime);

                if (displayProgress >= 1f)
                {
                    _sceneLoadUI.UpdateLoadingUI(1.0f);
                    break;
                }
            }

            _sceneLoadUI.UpdateLoadingUI(displayProgress);

            yield return null;
        }

        yield return StartCoroutine(CompleteSceneLoading());
    }

    private IEnumerator CompleteSceneLoading()
    {
        _sceneLoadUI.CompleteSceneLoading();

        yield return new WaitForSeconds(0.3f);

        _isLoading = false;
        _levelSceneOperation = null;
        _gameLogicOperation = null;
        _inGameUiOperation = null;
        GameManager.Instance.IsTitle = false;
    }
}