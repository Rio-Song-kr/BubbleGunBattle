using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    private static AudioSourcePool _instance;
    public static AudioSourcePool Instance => _instance;

    //# AudioSource는 MonoBehaviour를 상속받지 않기게, 빈 게임 Object에 AudioSourceController를 추가하여 Prefab화
    [SerializeField] private GameObject _audioSourcePrefab;

    public PoolManager<AudioSourceController> Pool;

    private void Awake()
    {
        _instance = this;
        var prefab = _audioSourcePrefab.GetComponent<AudioSourceController>();
        Pool = new PoolManager<AudioSourceController>(prefab, 10, 20, transform);
    }
}