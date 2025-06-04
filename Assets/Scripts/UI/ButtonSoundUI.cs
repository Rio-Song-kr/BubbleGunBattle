using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundUI : MonoBehaviour
{
    private void OnEnable()
    {
        var children = GetComponentsInChildren<Button>(true);

        foreach (var child in children)
        {
            child.onClick.AddListener(() => GameManager.Instance.Audio.Play2DSFX(AudioClipName.ButtonClick));
        }
    }

    private void OnDisable()
    {
        var children = GetComponentsInChildren<Button>(true);

        foreach (var child in children)
        {
            child.onClick.RemoveListener(() => GameManager.Instance.Audio.Play2DSFX(AudioClipName.ButtonClick));
        }
    }
}