using System;
using UnityEngine;
using Object = System.Object;

public class Player : MonoBehaviour, IDamageable
{
    private PlayerStats _stats;

    // Action 자료형 : 함수(들)을 저장할 수 있다.
    public event Action OnDamaged;
    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_stats.Health <= 0) return;

        _stats.Health -= damageInfo.Value;

        OnDamaged?.Invoke();

        if (_stats.Health <= 0f)
        {
            Debug.Log("플레이어 죽음");
            GameManager.Instance.GameOver();
        }
    }
}