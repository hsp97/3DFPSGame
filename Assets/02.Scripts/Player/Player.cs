using System;
using UnityEngine;
using Object = System.Object;

public class Player : MonoBehaviour, IDamageable
{
    private PlayerStats _stats;
    private Animator _animator;
    private int _injuredLayerIndex;

    private WeaponMode _weaponMode = WeaponMode.Normal;
    public WeaponMode WeaponMode => _weaponMode;

    // Action 자료형 : 함수(들)을 저장할 수 있다.
    public event Action OnDamaged;
    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
        _injuredLayerIndex = _animator.GetLayerIndex("Injured Layer");
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_stats.Health <= 0) return;

        _stats.Health -= damageInfo.Value;

        // 체력에 따라 Injured Layer의 가중치를 바꿔준다.
        float healthPercent = _stats.Health / _stats.MaxHealth; // 0~1 
        float injured = Mathf.InverseLerp(0f, 1f, healthPercent);

        // index(레이어 인덱스), 가중치
        _animator.SetLayerWeight(_injuredLayerIndex, injured);

        OnDamaged?.Invoke();

        if (_stats.Health <= 0f)
        {
            GameManager.Instance.GameOver();
        }

        _animator.SetTrigger("Hit");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponMode = WeaponMode.Normal;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponMode = WeaponMode.Sniper;
        }
    }
}