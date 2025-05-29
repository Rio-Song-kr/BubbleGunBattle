using UnityEngine;

[CreateAssetMenu(fileName = "BubbleDataSO", menuName = "Scriptable Objects/BubbleDataSO")]
public class BubbleDataSO : ScriptableObject
{
    public float Force = 15f;
    public float GrowDuration = 1f;
    public float StartScale = 0.1f;
    public float EndScale = 2f;
    public float DestroyDelay = 3f;
    public GameObject PopEffect;
}