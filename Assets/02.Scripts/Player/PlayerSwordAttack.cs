using System;
using UnityEngine;
using UnityEngine.VFX;

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

    // 콤보의 휘두름 한 번에 맞춘 베기 이펙트 값. 모든 위치/방향은 Player 기준 로컬 좌표다.
    [Serializable]
    private class SlashPreset
    {
        public Vector3 Center; // 칼끝이 그리는 원호의 중심
        public Vector3 Normal; // 원호가 놓인 평면의 법선
        public Vector3 StartDirection; // 중심에서 원호 시작점으로 향하는 방향
        public float Radius = 1.5f;
        public float Width = 0.63f;
        public float SweepDegrees = 137f;
        public float StartDegrees = 0f;
        public float TiltDegrees = 0f; // Player 정면 축을 기준으로 호를 세우는 보정(양수일수록 오른쪽으로 기운 호가 곧게 선다)
    }

    [Header("이펙트")]
    [SerializeField] private FireSlashVFX.FireSlash _fireSlash; // Katana에 붙은 칼끝 궤적 추적 이펙트. 있으면 아래 ClaudeSlash 방식 대신 쓴다.
    [SerializeField] private VisualEffect _slashEffect; // 휘두를 때 한 번 재생하는 베기 이펙트
    [SerializeField] private float _slashRadius = 1.5f; // 이펙트 반지름 (프리셋이 없을 때)
    [SerializeField] private float _slashWidth = 0.63f; // 이펙트 폭 (프리셋이 없을 때)
    [SerializeField] private float _slashSweepDegrees = 137f; // 휘두르는 각도 (프리셋이 없을 때)
    [SerializeField] private float _slashStartDegrees = 0f; // 시작 각도 (프리셋이 없을 때)

    // 콤보의 휘두름 순서대로 하나씩 쓴다. 애니메이션의 칼끝 궤적에서 계산한 값이다.
    [SerializeField] private SlashPreset[] _slashPresets;

    // 이펙트가 원호를 그리는 평면/방향 규칙이 프리셋 계산과 다르면 이 값으로 한 번에 보정한다.
    // 프리셋의 방향은 이펙트의 로컬 Y축을 법선, 로컬 Z축을 시작 방향으로 가정해 만든다.
    [SerializeField] private Vector3 _effectAxisOffset = Vector3.zero;

    // 1인칭에서도 보이도록 이펙트를 플레이어(머리) 쪽으로 끌어당긴다. 0이면 계산한 궤적 그대로, 1이면 머리 위치다.
    [SerializeField, Range(0f, 1f)] private float _slashPullIn = 0.5f;
    [SerializeField] private Vector3 _slashAnchor = new Vector3(0f, 0.8f, 0.3f); // 끌어당길 기준점 (Player 로컬, 1인칭 카메라 위치)
    [SerializeField] private float _slashRadiusScale = 0.85f; // 끌어당긴 뒤 이펙트 반지름 배율

    private int _slashIndex = 0; // 이번에 쓸 프리셋 번호

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponentInChildren<Animator>();

        // 이벤트가 켜기 전까지는 꺼 둔다.
        _katanaCollider.enabled = false;

        if (_slashEffect != null)
        {
            _slashEffect.SetFloat("Radius", _slashRadius);
            _slashEffect.SetFloat("Width", _slashWidth);
            _slashEffect.SetFloat("SweepDegrees", _slashSweepDegrees);
            _slashEffect.SetFloat("StartDegrees", _slashStartDegrees);
        }
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
        // 휘두르는 순간(애니메이션 이벤트)에 베기 이펙트를 재생한다.
        PlaySlashEffect();

        // 부채꼴 방식은 휘두르는 순간에 한 번 판정하고, 콜라이더 방식은 칼날 콜라이더를 켠다.
        if (_hitMode == MeleeHitMode.Fan)
        {
            FanAttack();
            return;
        }

        // 카타나 콜라이더 활성화
        _katanaCollider.enabled = true;
    }

    private void PlaySlashEffect()
    {
        // 휘두르는 순간(애니메이션 이벤트)에 칼끝 궤적을 읽어 이펙트를 낸다.
        if (_fireSlash != null)
        {
            _fireSlash.Play();
            return;
        }

        if (_slashEffect == null)
        {
            return;
        }

        if (_slashPresets != null && _slashPresets.Length > 0)
        {
            SlashPreset preset = _slashPresets[_slashIndex % _slashPresets.Length];
            _slashIndex++;

            // 이펙트를 칼끝이 그리는 원호의 중심에 두고, 원호의 평면과 시작 방향에 맞춰 돌린다.
            Transform fx = _slashEffect.transform;
            fx.localPosition = Vector3.Lerp(preset.Center, _slashAnchor, _slashPullIn);
            Quaternion tilt = Quaternion.AngleAxis(preset.TiltDegrees, Vector3.forward);
            fx.localRotation = Quaternion.LookRotation(tilt * preset.StartDirection, tilt * preset.Normal) * Quaternion.Euler(_effectAxisOffset);

            _slashEffect.SetFloat("Radius", preset.Radius * _slashRadiusScale);
            _slashEffect.SetFloat("Width", preset.Width);
            _slashEffect.SetFloat("SweepDegrees", preset.SweepDegrees);
            _slashEffect.SetFloat("StartDegrees", preset.StartDegrees);
        }

        _slashEffect.SendEvent("OnPlay");
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

        // 공격을 멈추면 다음 공격은 콤보의 첫 번째 휘두름부터 시작한다.
        if (!isSlashing)
        {
            _slashIndex = 0;
        }

        // 공격이 끊겨 UnActivate 이벤트를 못 만나도 콜라이더가 켜진 채 남지 않게 한다.
        if (!isSlashing && _katanaCollider.enabled)
        {
            _katanaCollider.enabled = false;
        }
    }
}
