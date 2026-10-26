using System;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private Zombie _zombie;

    private void Awake()
    {
        _zombie = GetComponent<Zombie>();
    }

    private void Start()
    {
        if (_zombie != null)
        {
            _zombie.OnDamaged += RefreshHealth;
            // 풀에서 재사용될 때 이전 생의 체력바가 남지 않게 다시 채운다.
            _zombie.OnSpawned += RefreshHealth;
        }
    }

    private void LateUpdate()
    {
        // 빌보드: 체력바가 카메라를 쳐다보게 한다.
        _slider.transform.forward = Camera.main.transform.forward;
    }

    private void RefreshHealth()
    {
        _slider.value = _zombie.Health / _zombie.MaxHealth;
    }
}