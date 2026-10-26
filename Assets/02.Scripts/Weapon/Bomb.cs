using UnityEngine;
using UnityEngine.Pool;

// 무언가에 부딛히면 폭발 이펙트를 남기고 사라지는 폭탄
public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;
    [SerializeField] private float _damage = 100f;
    [SerializeField] private float _explosionRange = 5f;

    private IObjectPool<Bomb> _pool;

    // 풀에서 생성될 때 한 번 호출된다. 사라질 때 이 풀로 돌아간다.
    public void SetPool(IObjectPool<Bomb> pool)
    {
        _pool = pool;
    }

    // 다른 콜라이더와 부딛힌 순간 실행되는 메서드
    private void OnCollisionEnter(Collision collision)
    {
        // 같은 물리 프레임에 여러 콜라이더와 부딪히면 여러 번 호출되므로, 이미 반납된 폭탄은 무시한다.
        if (!gameObject.activeSelf) return;

        ParticleSystem effect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        effect.Play();

        ApplyAreaDamage();

        // 폭탄을 삭제하지 않고 풀에 반납한다.
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ApplyAreaDamage()
    {
        // 0. 주위에 있는 드럼통들을 모두 찾는다.
        // Physics.OverlapSpere(원점, 반지름, 찾을 레이어들)
        Collider[] colliders =
            Physics.OverlapSphere(transform.position, _explosionRange, LayerMask.GetMask("Drum", "Zombie"));

        // 1. 드럼통과 좀비들에게 대미지를 준다.
        foreach (Collider col in colliders)
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Value = _damage;
            damageInfo.HitPoint = col.ClosestPoint(transform.position);     // 폭발 중심에서 가장 가까운 콜라이더 표면 위치
            damageInfo.HitNormal = (transform.position - col.ClosestPoint(transform.position)).normalized;
            col.GetComponent<IDamageable>()?.TakeDamage(damageInfo);
        }
    }
}