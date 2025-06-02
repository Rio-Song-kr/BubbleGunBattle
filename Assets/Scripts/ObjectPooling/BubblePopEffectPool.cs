using UnityEngine;

public class BubblePopEffectPool : MonoBehaviour
{
    private static BubblePopEffectPool _instance;
    public static BubblePopEffectPool Instance => _instance;

    public PoolManager<BubblePopEffect> Pool;

    private void Awake()
    {
        _instance = this;
        Pool = new PoolManager<BubblePopEffect>(Resources.Load<BubblePopEffect>("BubblePopEffect"));
    }
}