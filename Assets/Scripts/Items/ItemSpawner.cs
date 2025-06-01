using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _items;
    [SerializeField] private float _spawnMaxTime;
    [SerializeField] private float _spawnMinTime;
    [SerializeField] private Vector2 _minBackgroundSize;
    [SerializeField] private Vector2 _maxBackgroundSize;
    [SerializeField] private float _itemDistance;

    private float _lastSpawnTime;
    private float _spawnTime;
    private ItemManager _itemManager;

    private void OnEnable()
    {
        _itemManager = GameManager.Instance.ItemManager;
    }

    private void Start()
    {
        _spawnTime = Random.Range(_spawnMinTime, _spawnTime);
        _lastSpawnTime = 0f;
    }

    private void Update()
    {
        if (Time.time < _lastSpawnTime + _spawnTime) return;

        _lastSpawnTime = Time.time;
        _spawnTime = Random.Range(_spawnMinTime, _spawnTime);
        Spawn();
    }

    private void Spawn()
    {
        var spawnPosition = GetRandomPointOnNavMesh(_itemDistance);
        spawnPosition += Vector3.up * 0.5f;

        //# 아이템 중 하나를 무작위로 생성
        var selectedItem = _items[Random.Range(0, _items.Length)];
        var item = Instantiate(selectedItem, spawnPosition, Quaternion.identity);

        _itemManager.AddItem(item);
    }

    private Vector3 GetRandomPointOnNavMesh(float distance)
    {
        NavMeshHit hit = default;
        int count = 0;

        while (count < 3)
        {
            var randomPosition = new Vector3(
                Random.Range(_minBackgroundSize.x, _maxBackgroundSize.x),
                0f,
                Random.Range(_minBackgroundSize.y, _maxBackgroundSize.y)
            );

            //# RandomPosition에서 distance 내에서 샘플을 추출
            NavMesh.SamplePosition(randomPosition, out hit, distance, NavMesh.AllAreas);

            //# 기존에 생성된 아이템과의 거리를 확인하여 생성
            if (_itemManager.CanCreate(hit.position, distance)) break;
            count++;
        }
        if (count >= 3)
        {
            return Vector3.zero;
        }
        return hit.position;
    }
}