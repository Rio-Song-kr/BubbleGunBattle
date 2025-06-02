using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public WaitForSeconds DestroyDelay = new WaitForSeconds(5f);
    public ItemSpawner Spawner;
}