using UnityEngine;

public class ItemPool<T> where T : Item
{
    public PoolManager<T> Pool;

    public void SetPool(GameObject prefab)
    {
        var component = prefab.GetComponent<T>();
        Pool = new PoolManager<T>(component);
    }
}