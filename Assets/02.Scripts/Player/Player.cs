using System;
using UnityEngine;
using Object = System.Object;

public class Player : MonoBehaviour, IDamageable
{
    private PlayerStats _stats;
    private Animator _animator;
    private int _injuredLayerIndex;
    private int _swordLayerIndex;

    [SerializeField] private GameObject _katana; // 검 모드일 때만 손에 보이는 검 모델

    private WeaponMode _weaponMode = WeaponMode.Normal;
    public WeaponMode WeaponMode => _weaponMode;

    // Action 자료형 : 함수(들)을 저장할 수 있다.
    public event Action OnDamaged;
    public event Action OnWeaponModeChanged;
    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
        _injuredLayerIndex = _animator.GetLayerIndex("Injured Layer");
        _swordLayerIndex = _animator.GetLayerIndex("Sword Layer");

        ApplySwordVisual();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_stats.Health <= 0) return;

        _stats.Health -= damageInfo.Value;

        // 체력에 따라 Injured Layer의 가중치를 바꿔준다.
        float healthPercent = _stats.Health / _stats.MaxHealth; // 0~1 
        float injured = Mathf.InverseLerp(0.5f, 0f, healthPercent);

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
            SetWeaponMode(WeaponMode.Normal);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeaponMode(WeaponMode.Sniper);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeaponMode(WeaponMode.Sword);
        }
    }

    private void SetWeaponMode(WeaponMode mode)
    {
        if (_weaponMode == mode) return;

        _weaponMode = mode;

        ApplySwordVisual();

        OnWeaponModeChanged?.Invoke();
    }

    // 검 모드일 때만 검 자세와 공격(Sword Layer의 Idle, Attack)을 켜고, 검 모델도 그때만 보인다.
    private void ApplySwordVisual()
    {
        bool isSword = _weaponMode == WeaponMode.Sword;

        _animator.SetLayerWeight(_swordLayerIndex, isSword ? 1f : 0f);
        _katana.SetActive(isSword);
    }
}