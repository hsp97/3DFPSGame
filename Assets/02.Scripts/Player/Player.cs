using System;
using UnityEngine;
using Object = System.Object;

public class Player : MonoBehaviour, IDamageable
{
    private PlayerStats _stats;
    private Animator _animator;
    // Action 자료형 : 함수(들)을 저장할 수 있다.
    public event Action OnDamaged;
    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_stats.Health <= 0) return;

        _stats.Health -= damageInfo.Value;

        OnDamaged?.Invoke();

        if (_stats.Health <= 0f)
        {
            GameManager.Instance.GameOver();
        }

        _animator.SetTrigger("Hit");
    }
}