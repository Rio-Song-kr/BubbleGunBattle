using UnityEngine;

public interface ITransformAdjustable
{
    public void SetPosition(Vector3 position);
    public Vector3 GetPosition();
    public void SetLocalScale(Vector3 localScale);
    public void SetParent(Transform parent, bool worldPositionStays = true);
}