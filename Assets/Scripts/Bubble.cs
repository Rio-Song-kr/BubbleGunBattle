using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private BubbleDataSO _bubbleData;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one * _bubbleData.StartScale;
        Move();
    }

    private void Start()
    {
        StartCoroutine(GrowBubble());
        Invoke(nameof(DestroyBubble), _bubbleData.DestroyDelay);
    }

    private void Move()
    {
        _rigidbody.AddForce(transform.forward * _bubbleData.Force, ForceMode.Impulse);
    }

    private IEnumerator GrowBubble()
    {
        float time = 0f;

        while (time < _bubbleData.GrowDuration)
        {
            float scale = Mathf.Lerp(_bubbleData.StartScale, _bubbleData.EndScale, time / _bubbleData.GrowDuration);
            transform.localScale = Vector3.one * scale;

            time += Time.deltaTime;

            yield return null;
        }

        transform.localScale = Vector3.one * _bubbleData.EndScale;
    }

    private void DestroyBubble()
    {
        //# 추후 Object Pooling 적용 시 수정될 예정
        var effect = Instantiate(_bubbleData.PopEffect, transform.position, transform.rotation);
        Destroy(effect, 1f);
        Destroy(gameObject);
    }

    private void CancelDestroy()
    {
        CancelInvoke(nameof(DestroyBubble));
    }
}