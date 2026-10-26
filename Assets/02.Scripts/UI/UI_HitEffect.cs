using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_HitEffect : MonoBehaviour
{
    [SerializeField] private float _flashTime = 0.1f;
    [SerializeField] private float _fadeTime = 0.3f;
    [SerializeField] private float _alpha = 1f;
    private Image _image;
    private Player _player;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
    }

    private void Start()
    {
        _player = GameObject.FindAnyObjectByType<Player>();
        if (_player != null)
        {
            _player.OnDamaged += OnPlayerDamaged;
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnDamaged -= OnPlayerDamaged;
        }
    }

    private void OnPlayerDamaged()
    {
        Flash(_alpha);
    }

    public void Flash(float alpha)
    {
        SetAlpha(alpha);
        StartCoroutine(Flash_Coroutine());
    }

    private IEnumerator Flash_Coroutine()
    {
        _image.enabled = true;
        yield return new WaitForSeconds(_flashTime);
        yield return Fade_Coroutine();
        _image.enabled = false;
    }

    private IEnumerator Fade_Coroutine()
    {
        float time = 0f;
        float startAlpha = _image.color.a;
        while (time < _fadeTime)
        {
            float alpha = Mathf.Lerp(startAlpha, 0f, time / _fadeTime);
            time += Time.deltaTime;
            SetAlpha(alpha);

            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}