using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BubbleDataSO", menuName = "Scriptable Objects/BubbleDataSO")]
public class BubbleDataSO : ScriptableObject
{
    public float BubbleShootForce = 10f;
    public float ObjectPushForce = 5f;

    public float GrowDuration = 0.5f;
    public WaitForSeconds UnTrappedReleaseDelay = new WaitForSeconds(3f);
    public WaitForSeconds PlayerTrappedReleaseDelay = new WaitForSeconds(1f);
    public WaitForSeconds ItemTrappedReleaseDelay = new WaitForSeconds(2f);

    public float StartScale = 0.1f;
    public float EndScale = 2f;
    [FormerlySerializedAs("PlayerEncapsulateScale")]
    public float PlayerEncapsulatedScale = 3f;
    [FormerlySerializedAs("ItemEncapsulateScale")]
    public float ItemEncapsulatedScale = 2f;

    public GameObject PopEffect;
    public float PopEffectDuration = 1f;

    public float PlayerYOffset = -0.8f;
    public float ItemYOffset = -0.5f;

    public float Drag = 3f;

    public PhysicMaterial RollingMaterial;
}