using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("체력")]
    private float _maxHealth = 100f; // 최대 체력
    private float _health; // 현재 체력
    private float _healthRegen = 0f; // 1초에 회복하는 체력

    [Header("스태미나")]
    private float _maxStamina = 100f; // 최대 스태미나
    private float _stamina; // 현재 스태미나
    private float _staminaRegen = 15f; // 1초에 회복하는 스태미나

    [Header("능력치")]
    public float Damage = 10f; // 공격력
    public float MoveSpeed = 7f; // 걷기 속도 (m/s)
    public float SprintSpeed = 12f; // 질주 속도 (m/s)
    public float JumpPower = 7f; // 점프력 (m/s)

    public float MaxHealth
    {
        get => _maxHealth;
    }

    public float Health
    {
        get => _health;
        set { _health = value; }
    }
    public float MaxStamina
    {
        get => _maxStamina;
    }

    public float Stamina
    {
        get => _stamina;
        set { _stamina = value; }
    }

    private void Awake()
    {
        _health = _maxHealth;
        _stamina = _maxStamina;
    }

    private void Update()
    {
        _health = Mathf.Min(_maxHealth, _health + _healthRegen * Time.deltaTime);
        _stamina = Mathf.Min(_maxStamina, _stamina + _staminaRegen * Time.deltaTime);
    }
}