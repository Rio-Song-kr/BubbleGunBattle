using System.Collections;
using UnityEngine;

public class BubblePopEffect : MonoBehaviour
{
    private ParticleSystem _particle;
    private WaitForSeconds _wait = new WaitForSeconds(1f);

    private void Awake() => _particle = GetComponent<ParticleSystem>();

    private void OnDisable()
    {
        _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _particle.time = 0f;
    }

    public void Play()
    {
        _particle.Play();
        StartCoroutine(ReleaseCoroutine());
    }

    private IEnumerator ReleaseCoroutine()
    {
        yield return _wait;
        BubblePopEffectPool.Instance.Pool.Release(this);
    }
}