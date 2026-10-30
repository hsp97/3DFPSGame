// 캐릭터 공격에 베기 이펙트를 붙이는 스크립트
// - Space 또는 마우스 왼쪽 클릭: 공격
// - autoRepeat > 0 이면 그 간격(초)마다 자동으로 공격 (미리보기용, 0이면 끔)
// - 공격 상태의 재생 위치가 vfxAtNormalizedTime 을 지나는 순간 VFX 에 OnPlay 이벤트를 보냄
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ClaudeSlash
{
    public sealed class ClaudeSlashAttack : MonoBehaviour
    {
        public Animator animator;
        public VisualEffect slash;
        public string attackState = "Attack";
        [Range(0f, 1f)] public float vfxAtNormalizedTime = 0.3f;
        [Min(0f)] public float autoRepeat = 2f;
        [HideInInspector] public bool logEvents;
        [Tooltip("칼끝 (궤적 측정용)")] public Transform tip;
        [HideInInspector] public bool record;
        private readonly List<(float t, float norm, Vector3 p)> samples = new List<(float, float, Vector3)>();

        [Header("칼 궤적에 맞춘 이펙트 크기 (VFX Exposed 파라미터)")]
        public float radius = 1.5f;
        public float width = 0.63f;
        public float sweepDegrees = 137f;
        public float startDegrees = 0f;

        private bool fired = true;
        private float nextAuto;

        private void Start()
        {
            ApplyShape();
            nextAuto = Time.time + 0.5f;
        }

        public void ApplyShape()
        {
            if (slash == null) return;
            slash.SetFloat("Radius", radius);
            slash.SetFloat("Width", width);
            slash.SetFloat("SweepDegrees", sweepDegrees);
            slash.SetFloat("StartDegrees", startDegrees);
        }

        private void Update()
        {
            bool pressed = false;
#if ENABLE_INPUT_SYSTEM
            pressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);
#endif
            if (pressed || (autoRepeat > 0f && Time.time >= nextAuto)) Attack();

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!fired && state.shortNameHash == Animator.StringToHash(attackState) && state.normalizedTime >= vfxAtNormalizedTime)
            {
                fired = true;
                slash.SendEvent("OnPlay");
                if (logEvents) Debug.Log("CLAUDE slash fired at " + state.normalizedTime.ToString("F2"));
            }
        }

        private void LateUpdate()
        {
            if (!record || tip == null) return;
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.shortNameHash == Animator.StringToHash(attackState))
                samples.Add((Time.time, state.normalizedTime, transform.InverseTransformPoint(tip.position)));
        }

        // 플레이 중 기록한 칼끝 궤적으로 원호를 맞춰 파일에 씀: 가장 빠른 구간 앞뒤로 속도 30% 이상인 범위
        public string FitText()
        {
            if (samples.Count < 5) return "";
            var sp = new List<float> { 0 };
            for (int i = 1; i < samples.Count; i++) sp.Add((samples[i].p - samples[i - 1].p).magnitude / Mathf.Max(1e-4f, samples[i].t - samples[i - 1].t));
            int peak = sp.IndexOf(sp.Max()), s0 = peak, s1 = peak;
            while (s0 > 1 && sp[s0 - 1] > sp[peak] * 0.3f) s0--;
            while (s1 < sp.Count - 1 && sp[s1 + 1] > sp[peak] * 0.3f) s1++;
            Vector3 a = samples[s0 - 1].p, b = samples[(s0 + s1) / 2].p, c = samples[s1].p;
            Vector3 n = Vector3.Cross(b - a, c - b).normalized;
            Vector3 ab = b - a, ac = c - a, nn = Vector3.Cross(ab, ac);
            Vector3 center = a + (Vector3.Cross(nn, ab) * ac.sqrMagnitude + Vector3.Cross(ac, nn) * ab.sqrMagnitude) / (2f * nn.sqrMagnitude);
            float sweep = Vector3.SignedAngle(a - center, b - center, n) + Vector3.SignedAngle(b - center, c - center, n);
            string V(Vector3 v) => string.Join(",", new[] { v.x, v.y, v.z }.Select(x => x.ToString("F4", CultureInfo.InvariantCulture)));
            string F(float x) => x.ToString("F4", CultureInfo.InvariantCulture);
            return $"center={V(center)}\nnormal={V(n)}\nstart={V((a - center).normalized)}\nradius={F((a - center).magnitude)}\nsweep={F(sweep)}\ntrigger={F(samples[s0 - 1].norm)}\nswing={F(samples[s1].t - samples[s0 - 1].t)}\n";
        }

        public void Attack()
        {
            animator.Play(attackState, 0, 0f);
            fired = false;
            nextAuto = Time.time + autoRepeat;
        }
    }
}
