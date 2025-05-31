using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BubbleDataSO", menuName = "Scriptable Objects/BubbleDataSO")]
public class BubbleDataSO : ScriptableObject
{
    public float BubbleShootForce = 10f;
    public float ObjectPushForce = 5f;

    public float GrowDuration = 0.5f;
    public WaitForSeconds UnTrappedDestroyDelay = new WaitForSeconds(3f);
    public WaitForSeconds TrappedDestroyDelay = new WaitForSeconds(1f);
    public WaitForSeconds ItemTrappedDestroyDelay = new WaitForSeconds(2f);

    public float StartScale = 0.1f;
    public float EndScale = 2f;
    public float PlayerEncapsulateScale = 3f;
    public float ItemEncapsulateScale = 2f;

    public GameObject PopEffect;
    public float PopEffectDuration = 1f;

    public float PlayerYOffset = -0.8f;
    public float ItemYOffset = -0.5f;

    public float Drag = 3f;

    public PhysicMaterial RollingMaterial;
}