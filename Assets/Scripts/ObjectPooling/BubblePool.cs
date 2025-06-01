using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePool : MonoBehaviour
{
    private static BubblePool _instance;
    public static BubblePool Instance => _instance;

    public PoolManager<Bubble> Pool;

    private void Awake()
    {
        _instance = this;
        Pool = new PoolManager<Bubble>(Resources.Load<Bubble>("Bubble"));
    }
}