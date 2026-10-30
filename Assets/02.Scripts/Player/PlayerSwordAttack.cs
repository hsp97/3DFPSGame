using System;
using UnityEngine;

// 검 모드에서 공격키(마우스 왼쪽 버튼)를 누르고 있는 동안 Slash 애니메이션을 재생하고 싶다.
public class PlayerSwordAttack : MonoBehaviour
{
    private static readonly int SlashHash = Animator.StringToHash("Slash");

    private Player _player;
    private Animator _animator;
    [SerializeField] private MeleeHitMode _hitMode = MeleeHitMode.Collider;  // 맞힐 대상을 고르는 방법
    [SerializeField] private Collider _katanaCollider;

    [Header("부채꼴")]
    [SerializeField] private float _range = 2f;
    [SerializeField] private float _angle = 90f;
    [SerializeField] private LayerMask _targetMask;
    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponentInChildren<Animator>();

        // 이벤트가 켜기 전까지는 꺼 둔다.
        _katanaCollider.enabled = false;
    }

    public void FanAttack()
    {
        if (_hitMode != MeleeHitMode.Fan) return;

        Vector3 origin = _player.transform.position;

        Collider[] colliders = Physics.OverlapSphere(origin, _range, _targetMask);
        foreach (Collider col in colliders)
        {

            Vector3 hitPoint = col.ClosestPoint(origin);
            Vector3 direction = (hitPoint - origin);
            direction.y = 0f;
            float angle = Vector3.Angle(_player.transform.forward, direction);
            if (angle > _angle / 2f) continue;

            if (Physics.Linecast(origin, hitPoint, LayerMask.NameToLayer("Wall"))) continue;

            if (col.TryGetComponent(out IDamageable damageable))
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.Value = 25f;
                damageInfo.HitPoint = hitPoint;
                damageInfo.HitNormal = (origin - hitPoint).normalized;
                damageable.TakeDamage(damageInfo);
            }
        }
    }
    public void ActivateCollider()
    {
        // 부채꼴 방식은 휘두르는 순간에 한 번 판정하고, 콜라이더 방식은 칼날 콜라이더를 켠다.
        if (_hitMode == MeleeHitMode.Fan)
        {
            FanAttack();
            return;
        }

        // 카타나 콜라이더 활성화
        _katanaCollider.enabled = true;
    }

    public void UnActivateCollider()
    {
        // 카타나 콜라이더 비활성화
        _katanaCollider.enabled = false;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        bool isSlashing = _player.WeaponMode == WeaponMode.Sword && !CursorManager.IsUiCursor && Input.GetButton("Fire1");
        _animator.SetBool(SlashHash, isSlashing);

        // 공격이 끊겨 UnActivate 이벤트를 못 만나도 콜라이더가 켜진 채 남지 않게 한다.
        if (!isSlashing && _katanaCollider.enabled)
        {
            _katanaCollider.enabled = false;
        }
    }
}
