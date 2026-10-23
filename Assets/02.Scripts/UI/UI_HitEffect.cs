using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_HitEffect : MonoBehaviour
{
    [SerializeField] private float _flashTime = 0.2f;

    private Image _image;
    private float _fadeDuration = 0.8f;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
    }

    public void Flash(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
        StartCoroutine(Flash_Coroutine(alpha));
    }

    private IEnumerator Flash_Coroutine(float alpha)
    {
        _image.enabled = true;
        yield return StartCoroutine(Fade_Coroutine(alpha));
        _image.enabled = false;
    }

    private IEnumerator Fade_Coroutine(float alpha)
    {
        float time = 0f;
        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            Color color = _image.color;
            color.a = Mathf.Lerp(alpha, 0f, time / _fadeDuration);
            _image.color = color;

            yield return null;
        }
    }
}