using System;
using System.Collections.Generic;
using UnityEngine;

namespace FireSlashVFX
{
    /// <summary>
    /// PF_VFX_07_FireSlash를 원하는 위치·각도로 재생하는 컴포넌트.
    ///
    /// Manual       : Inspector의 프리셋(위치/좌우 회전/기울기/방향/반지름/속도)대로 재생한다.
    ///                Scene 뷰에 호 모양 가이드(Gizmo)가 그려지니 칼 궤적에 겹쳐서 맞춘다.
    ///                애니메이션 이벤트에서 Play() 또는 PlayPreset(int)를 호출한다.
    /// FollowObject : 칼끝(Tip)의 움직임을 추적해서 그 궤적의 평면·방향·반지름·속도로 재생한다.
    ///                칼끝이 빨라지면 자동으로 나오거나(Auto Trigger), 애니메이션 이벤트의 Play()로 나온다.
    ///
    /// 이펙트 좌표 규칙: 루트 = 호의 중심, 루트 +Z = 호의 가운데, 루트 Y = 호 평면의 법선,
    /// 빛은 루트 -X 쪽에서 +X 쪽으로 200° 훑고 지나간다(약 0.2초). 반지름 1.4 = Scale 1.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("VFX/Fire Slash")]
    public sealed class FireSlash : MonoBehaviour
    {
        public enum SlashMode { Manual, FollowObject }

        [Serializable]
        public class Preset
        {
            public string name = "Slash";
            [Tooltip("Origin 기준 호 중심 위치 (로컬). 보통 캐릭터 가슴 높이.")]
            public Vector3 offset = new Vector3(0f, 1.1f, 0f);
            [Tooltip("호가 향하는 방향을 좌우로 돌린다 (도). 0 = Origin 정면.")]
            public float yaw;
            [Tooltip("호의 기울기 (도). 0 = 수평, -45 = 왼쪽 위→오른쪽 아래 내려베기, 45 = 올려베기, 90 = 세로.")]
            [Range(-180f, 180f)] public float tilt = -45f;
            [Tooltip("체크하면 오른쪽 → 왼쪽으로 휘두른다.")]
            public bool reverse;
            [Tooltip("호의 반지름 (m). 호 중심에서 칼끝까지 거리.")]
            [Min(0.05f)] public float radius = 1.4f;
            [Tooltip("재생 속도 배율. 스윙이 느린 애니메이션이면 1보다 작게.")]
            [Range(0.1f, 3f)] public float speed = 1f;
        }

        // 프리팹 기준값 (메시/파티클이 이 값으로 만들어져 있다)
        public const float MeshRadius = 1.4f;
        public const float ArcSpan = 200f;
        public const float NominalDegPerSec = 1000f; // 200°를 0.2초에

        [Header("Effect")]
        [Tooltip("PF_VFX_07_FireSlash 프리팹")]
        [SerializeField] private ParticleSystem effectPrefab;
        [Tooltip("동시에 겹쳐 재생할 수 있는 최대 개수 (연타용). 미리 만들어 두고 재사용한다.")]
        [SerializeField, Range(1, 8)] private int poolSize = 3;
        [SerializeField] private SlashMode mode = SlashMode.Manual;

        [Header("Manual")]
        [Tooltip("프리셋 위치/방향의 기준. 비워 두면 이 오브젝트. 보통 캐릭터 루트.")]
        [SerializeField] private Transform origin;
        [Tooltip("Play()는 0번, PlayPreset(n)은 n번 프리셋을 쓴다. 콤보마다 하나씩 만든다.")]
        [SerializeField] private Preset[] presets = { new Preset() };

        [Header("Follow Object")]
        [Tooltip("추적할 칼끝. 비워 두면 이 오브젝트.")]
        [SerializeField] private Transform tip;
        [Tooltip("Tip 기준 칼끝 위치 (로컬). 칼 오브젝트에 붙였다면 칼날 방향으로 칼 길이만큼.")]
        [SerializeField] private Vector3 tipOffset;
        [Tooltip("호의 중심 (어깨/가슴 본 등). 비워 두면 칼끝 궤적으로 원을 계산해서 중심을 찾는다.")]
        [SerializeField] private Transform pivot;
        [Tooltip("칼끝 속도가 Trigger Speed를 넘으면 자동으로 재생한다. 끄면 Play() 호출 때만 궤적을 읽는다.")]
        [SerializeField] private bool autoTrigger = true;
        [Tooltip("자동 재생 기준 칼끝 속도 (m/s).")]
        [SerializeField, Min(0.1f)] private float triggerSpeed = 8f;
        [Tooltip("궤적을 읽는 시간 (초). 길수록 방향이 정확하지만 늦게 나온다.")]
        [SerializeField, Range(0f, 0.15f)] private float detectTime = 0.03f;
        [Tooltip("한 번 나온 뒤 다시 나올 수 있기까지 최소 시간 (초).")]
        [SerializeField, Min(0f)] private float cooldown = 0.25f;
        [Tooltip("호의 시작 위치를 스윙 방향으로 옮긴다 (도). +면 앞으로, -면 뒤로.")]
        [SerializeField, Range(-180f, 180f)] private float angleOffset;
        [Tooltip("계산된 반지름에 곱하는 값.")]
        [SerializeField, Range(0.3f, 3f)] private float radiusMultiplier = 1f;
        [Tooltip("칼끝 회전 속도에 맞춰 재생 속도를 바꾼다.")]
        [SerializeField] private bool matchSwingSpeed = true;
        [SerializeField] private Vector2 speedRange = new Vector2(0.5f, 2f);

        [Header("Preview")]
        [Tooltip("Play 모드에서 프리셋을 순서대로 반복 재생한다 (Manual 각도 맞추기용).")]
        [SerializeField] private bool previewLoop;
        [SerializeField, Min(0.1f)] private float previewInterval = 1.2f;

        public SlashMode Mode { get => mode; set => mode = value; }

        private struct Sample
        {
            public float time;
            public Vector3 position;
        }

        private sealed class Instance
        {
            public ParticleSystem root;
            public ParticleSystem[] systems;
            public Transform orient;
        }

        private const float HistorySeconds = 0.5f;

        private readonly List<Instance> pool = new List<Instance>();
        private readonly List<Sample> samples = new List<Sample>();
        private readonly List<Vector3> pathBuffer = new List<Vector3>();
        private int nextInstance;
        private bool detecting;
        private float detectStart;
        private float lastFireTime = float.NegativeInfinity;
        private bool armed = true;
        private float previewTimer;
        private int previewIndex;

        // 마지막으로 계산한 호 (Gizmo 표시용)
        private bool hasLastArc;
        private Vector3 lastArcCenter;
        private Quaternion lastArcRotation;
        private float lastArcRadius;

        // ------------------------------------------------------------ 재생 API (애니메이션 이벤트용)

        /// Manual: 0번 프리셋으로 재생. FollowObject: 지금부터 칼끝 궤적을 읽고 재생.
        public void Play()
        {
            if (mode == SlashMode.FollowObject) BeginDetect(Time.time);
            else PlayPreset(0);
        }

        /// n번 프리셋으로 재생 (모드와 상관없이).
        public void PlayPreset(int index)
        {
            if (presets == null || presets.Length == 0) return;
            var p = presets[Mathf.Clamp(index, 0, presets.Length - 1)];
            GetPresetPose(p, out var center, out var rotation);
            PlayAt(center, rotation, p.radius, p.speed);
        }

        /// 직접 지정한 호로 재생. rotation의 +Z = 호의 가운데, +Y = 호 평면의 법선, -X → +X로 훑는다.
        public void PlayAt(Vector3 center, Quaternion rotation, float radius = MeshRadius, float speed = 1f)
        {
            if (effectPrefab == null)
            {
                Debug.LogWarning($"[FireSlash] {name}: Effect Prefab이 비어 있습니다.", this);
                return;
            }

            var fx = GetInstance(center, rotation);
            var t = fx.root.transform;
            t.SetPositionAndRotation(center, rotation);
            t.localScale = Vector3.one * (radius / MeshRadius);
            if (fx.orient != null) fx.orient.localRotation = Quaternion.identity;
            foreach (var ps in fx.systems)
            {
                var main = ps.main;
                main.simulationSpeed = speed;
            }
            fx.root.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fx.root.Play(true);

            hasLastArc = true;
            lastArcCenter = center;
            lastArcRotation = rotation;
            lastArcRadius = radius;
        }

        // ------------------------------------------------------------ 궤적 → 호 계산

        /// 칼끝 궤적(시간순 위치)으로 호를 계산한다. center를 주지 않으면 처음/가운데/끝 세 점의 외접원 중심을 쓴다.
        /// 결과 rotation은 PlayAt에 그대로 넣으면 호의 시작이 path[0] 방향이 된다.
        public static bool TrySolveArc(IReadOnlyList<Vector3> path, float duration, Vector3? center, float angleOffset,
            out Vector3 arcCenter, out Quaternion rotation, out float radius, out float degreesPerSecond)
        {
            arcCenter = default;
            rotation = Quaternion.identity;
            radius = 0f;
            degreesPerSecond = 0f;
            int n = path.Count;
            if (n < 3) return false;

            if (center.HasValue) arcCenter = center.Value;
            else if (!TryCircumcenter(path[0], path[n / 2], path[n - 1], out arcCenter)) return false;

            // 회전 방향이 들어간 평면 법선: 연속한 두 점이 중심과 이루는 외적의 합
            var normal = Vector3.zero;
            for (int i = 0; i < n - 1; i++)
                normal += Vector3.Cross(path[i] - arcCenter, path[i + 1] - arcCenter);
            if (normal.sqrMagnitude < 1e-10f) return false;
            normal.Normalize();

            var start = Vector3.ProjectOnPlane(path[0] - arcCenter, normal);
            var end = Vector3.ProjectOnPlane(path[n - 1] - arcCenter, normal);
            float rStart = start.magnitude, rEnd = end.magnitude;
            if (rStart < 0.05f || rEnd < 0.05f) return false;
            radius = (rStart + rEnd) * 0.5f;
            degreesPerSecond = duration > 1e-4f ? Vector3.Angle(start, end) / duration : 0f;

            var u = start / rStart;                       // 호가 시작하는 방향
            var w = Vector3.Cross(normal, u);             // 시작점에서 칼끝이 움직이는 방향
            float h = (ArcSpan * 0.5f + angleOffset) * Mathf.Deg2Rad;
            var forward = Mathf.Cos(h) * u + Mathf.Sin(h) * w;
            var right = -Mathf.Sin(h) * u + Mathf.Cos(h) * w;
            rotation = Quaternion.LookRotation(forward, Vector3.Cross(forward, right));
            return true;
        }

        private static bool TryCircumcenter(Vector3 a, Vector3 b, Vector3 c, out Vector3 center)
        {
            var ab = a - c;
            var bc = b - c;
            var n = Vector3.Cross(ab, bc);
            float d = 2f * n.sqrMagnitude;
            if (d < 1e-8f)
            {
                center = default;
                return false;
            }
            center = c + Vector3.Cross(ab.sqrMagnitude * bc - bc.sqrMagnitude * ab, n) / d;
            return true;
        }

        // ------------------------------------------------------------ 내부

        private void LateUpdate()
        {
            if (mode == SlashMode.FollowObject) TrackTip();

            if (previewLoop && mode == SlashMode.Manual && presets != null && presets.Length > 0)
            {
                previewTimer -= Time.deltaTime;
                if (previewTimer <= 0f)
                {
                    previewTimer = previewInterval;
                    PlayPreset(previewIndex++ % presets.Length);
                }
            }
        }

        private Vector3 TipPosition => (tip != null ? tip : transform).TransformPoint(tipOffset);

        private void TrackTip()
        {
            float now = Time.time;
            samples.Add(new Sample { time = now, position = TipPosition });
            int old = 0;
            while (old < samples.Count - 2 && samples[old].time < now - HistorySeconds) old++;
            if (old > 0) samples.RemoveRange(0, old);

            int count = samples.Count;
            float speed = 0f;
            if (count >= 2)
            {
                var a = samples[count - 2];
                var b = samples[count - 1];
                float dt = b.time - a.time;
                if (dt > 1e-5f) speed = Vector3.Distance(a.position, b.position) / dt;
            }
            if (speed < triggerSpeed * 0.5f) armed = true;

            if (detecting)
            {
                if (now - detectStart >= detectTime && CollectPath(detectStart) >= 3)
                {
                    detecting = false;
                    FireFromPath(now - detectStart);
                }
                return;
            }

            if (autoTrigger && armed && count >= 2 && speed >= triggerSpeed && now - lastFireTime >= cooldown)
                BeginDetect(samples[count - 2].time); // 속도가 넘기 직전 위치부터 궤적에 넣는다
        }

        private void BeginDetect(float startTime)
        {
            if (detecting) return;
            detecting = true;
            detectStart = startTime;
            armed = false;
        }

        private int CollectPath(float since)
        {
            pathBuffer.Clear();
            foreach (var s in samples)
                if (s.time >= since - 1e-5f) pathBuffer.Add(s.position);
            return pathBuffer.Count;
        }

        private void FireFromPath(float duration)
        {
            Vector3? center = pivot != null ? pivot.position : (Vector3?)null;
            if (!TrySolveArc(pathBuffer, duration, center, angleOffset, out var c, out var rot, out var radius, out var dps))
                return;

            float speed = matchSwingSpeed && dps > 0f
                ? Mathf.Clamp(dps / NominalDegPerSec, speedRange.x, speedRange.y)
                : 1f;
            lastFireTime = Time.time;
            PlayAt(c, rot, radius * radiusMultiplier, speed);
        }

        private Instance GetInstance(Vector3 position, Quaternion rotation)
        {
            foreach (var inst in pool)
                if (inst.root != null && !inst.root.IsAlive(true)) return inst;

            pool.RemoveAll(i => i.root == null);
            if (pool.Count < poolSize)
            {
                var root = Instantiate(effectPrefab, position, rotation);
                root.name = effectPrefab.name;
                var inst = new Instance
                {
                    root = root,
                    systems = root.GetComponentsInChildren<ParticleSystem>(true),
                    orient = root.transform.childCount > 0 ? root.transform.GetChild(0) : null,
                };
                pool.Add(inst);
                return inst;
            }
            // 모두 재생 중이면 가장 오래된 것을 다시 쓴다
            nextInstance %= pool.Count;
            return pool[nextInstance++];
        }

        private void GetPresetPose(Preset p, out Vector3 center, out Quaternion rotation)
        {
            var o = origin != null ? origin : transform;
            center = o.TransformPoint(p.offset);
            rotation = o.rotation * Quaternion.Euler(0f, p.yaw, 0f) * Quaternion.Euler(0f, 0f, p.tilt + (p.reverse ? 180f : 0f));
        }

        private void OnDestroy()
        {
            foreach (var inst in pool)
                if (inst.root != null) Destroy(inst.root.gameObject);
            pool.Clear();
        }

        private void OnValidate()
        {
            if (presets == null || presets.Length == 0) presets = new[] { new Preset() };
            speedRange.x = Mathf.Max(0.05f, speedRange.x);
            speedRange.y = Mathf.Max(speedRange.x, speedRange.y);
        }

        // ------------------------------------------------------------ Scene 뷰 가이드

        private void OnDrawGizmosSelected()
        {
            if (mode == SlashMode.Manual && presets != null)
            {
                for (int i = 0; i < presets.Length; i++)
                {
                    GetPresetPose(presets[i], out var c, out var r);
                    DrawArc(c, r, presets[i].radius, i == 0 ? new Color(1f, 0.45f, 0.1f) : new Color(1f, 0.8f, 0.2f, 0.6f));
                }
            }
            else if (mode == SlashMode.FollowObject)
            {
                var tipPos = TipPosition;
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(tipPos, 0.04f);
                if (pivot != null)
                {
                    Gizmos.DrawLine(pivot.position, tipPos);
                    Gizmos.DrawWireSphere(pivot.position, 0.06f);
                }
                for (int i = 1; i < samples.Count; i++)
                    Gizmos.DrawLine(samples[i - 1].position, samples[i].position);
                if (hasLastArc) DrawArc(lastArcCenter, lastArcRotation, lastArcRadius, new Color(1f, 0.45f, 0.1f));
            }
        }

        // 호 가이드: 시작점에 작은 구, 끝에 화살표 (빛이 훑는 방향)
        private static void DrawArc(Vector3 center, Quaternion rotation, float radius, Color color)
        {
            Gizmos.color = color;
            const int segments = 40;
            Vector3 Point(float deg) => center + rotation * new Vector3(Mathf.Sin(deg * Mathf.Deg2Rad), 0f, Mathf.Cos(deg * Mathf.Deg2Rad)) * radius;

            var prev = Point(-ArcSpan * 0.5f);
            Gizmos.DrawWireSphere(prev, radius * 0.04f);
            for (int s = 1; s <= segments; s++)
            {
                var p = Point(-ArcSpan * 0.5f + ArcSpan * s / segments);
                Gizmos.DrawLine(prev, p);
                prev = p;
            }
            var endDir = (Point(ArcSpan * 0.5f) - Point(ArcSpan * 0.5f - 5f)).normalized;
            var side = rotation * Vector3.up;
            var back = -endDir * radius * 0.12f;
            Gizmos.DrawLine(prev, prev + back + Vector3.Cross(endDir, side) * radius * 0.06f);
            Gizmos.DrawLine(prev, prev + back - Vector3.Cross(endDir, side) * radius * 0.06f);
            Gizmos.DrawLine(center, Point(0f));
        }
    }
}
