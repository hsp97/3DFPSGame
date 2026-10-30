using UnityEngine;

public class Katana : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _damage = 25f;
    private void OnTriggerEnter(Collider other)
    {
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
