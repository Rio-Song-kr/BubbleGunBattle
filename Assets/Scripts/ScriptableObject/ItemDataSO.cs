using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public int Score = 1;
    public WaitForSeconds DestroyDelay = new WaitForSeconds(5f);
}