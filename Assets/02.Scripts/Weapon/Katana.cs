using System;
using UnityEngine;

public class Katana : MonoBehaviour
{

    [SerializeField] private float _damage = 25f;

    private Player _player;
    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }


    private void OnTriggerEnter(Collider other)
    {
        // 검이 플레이어 자신의 몸에 닿아도 대미지를 주지 않는다.
        if (other.GetComponentInParent<Player>() != null)
        {
            return;
        }

        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Value = _damage;
            Vector3 hitPoint = other.ClosestPoint(_player.transform.position);
            damageInfo.HitPoint = hitPoint;
            damageInfo.HitNormal = (_player.transform.position - transform.position).normalized;

            damageable.TakeDamage(damageInfo);
        }
    }
}
