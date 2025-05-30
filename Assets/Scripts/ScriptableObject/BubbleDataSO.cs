using UnityEngine;

[CreateAssetMenu(fileName = "BubbleDataSO", menuName = "Scriptable Objects/BubbleDataSO")]
public class BubbleDataSO : ScriptableObject
{
    public float Force = 15f;

    public float GrowDuration = 1f;
    public WaitForSeconds UnTrappedDestroyDelay = new WaitForSeconds(3f);
    public WaitForSeconds TrappedDestroyDelay = new WaitForSeconds(1f);

    public float StartScale = 0.1f;
    public float EndScale = 2f;
    public float EncapsulateScale = 3f;

    public GameObject PopEffect;
    public float PopEffectDuration = 1f;

    public float PlayerYOffset = 0.3f;

    public float Drag = 2f;
}