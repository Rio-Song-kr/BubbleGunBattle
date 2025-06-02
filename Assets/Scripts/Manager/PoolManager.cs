using UnityEngine;
using UnityEngine.Pool;

public class PoolManager<T> where T : MonoBehaviour
{
    private readonly IObjectPool<T> _pool;

    public PoolManager(T prefab, int defaultCapacity = 5, int maxSize = 10)
    {
        _pool = new ObjectPool<T>
        (
            () => Object.Instantiate(prefab),
            obj => obj.gameObject.SetActive(true),
            obj => obj.gameObject.SetActive(false),
            obj => Object.Destroy(obj.gameObject),
            true,
            defaultCapacity,
            maxSize
        );
    }

    public T Get() => _pool.Get();

    public void Release(T obj) => _pool.Release(obj);
}