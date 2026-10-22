using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("체력")]
    public float MaxHealth = 100f;     // 최대 체력
    public float Health;               // 현재 체력
    public float HealthRegen = 0f;     // 1초에 회복하는 체력

    [Header("스태미나")]
    public float MaxStamina = 100f;    // 최대 스태미나
    public float Stamina;              // 현재 스태미나
    public float StaminaRegen = 15f;   // 1초에 회복하는 스태미나

    [Header("능력치")]
    public float Damage = 10f;         // 공격력
    public float MoveSpeed = 7f;       // 걷기 속도 (m/s)
    public float SprintSpeed = 12f;    // 질주 속도 (m/s)
    public float JumpPower = 7f;       // 점프력 (m/s)

    private void Awake()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
    }

    private void Update()
    {
        Health = Mathf.Min(MaxHealth, Health + HealthRegen * Time.deltaTime);
        Stamina = Mathf.Min(MaxStamina, Stamina + StaminaRegen * Time.deltaTime);
    }
}
