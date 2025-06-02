using System.Collections.Generic;
using UnityEngine;

public abstract class SceneUIController : MonoBehaviour
{
    protected Stack<GameObject> sceneUIStack = new Stack<GameObject>();

    protected virtual void Start()
    {
        GameManager.Instance.UI.SetCurrentSceneUI(this);
        ShowDefaultUI();
    }

    public abstract void ShowDefaultUI();
    public abstract void HideAllSceneUI();

    //# Scene 내부 UI Stack 관리
    protected void PushSceneUI(GameObject panel)
    {
        var currentPanel = GetCurrentActiveScenePanel();
        if (currentPanel != null)
        {
            sceneUIStack.Push(currentPanel);
            currentPanel.SetActive(false);
        }
        panel.SetActive(true);
    }

    protected void PopSceneUI()
    {
        if (sceneUIStack.Count > 0)
        {
            var previousPanel = sceneUIStack.Pop();
            HideAllSceneUI();
            previousPanel.SetActive(true);
        }
        else
        {
            ShowDefaultUI();
        }
    }

    protected abstract GameObject GetCurrentActiveScenePanel();
}