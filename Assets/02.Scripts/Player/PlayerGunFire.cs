using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


// 마우스 왼쪽 버튼을 누르면 바라보는 방향으로 총알을 발사하고 싶다.
public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffectPrefab;

    [Header("스탯")]
    [SerializeField] private float _reloadingTime = 3f;
    [SerializeField] private float _range = 100f; // 사거리
    [SerializeField] private float _damage = 10f; // 공격력
    [SerializeField] private float _fireInterval = 0.1f; // 발사 속도

    [SerializeField] private GameObject[] _muzzleFlashes;
    [SerializeField] private float _flashTime = 0.05f;

    [Header("총알 궤적")]
    [SerializeField] private Transform _muzzle; // 총구 위치
    [SerializeField] private PlayerBulletTrail _trailPrefab;
    [SerializeField] private Transform _trailParent; // 생성된 궤적을 모아둘 부모 오브젝트
    [SerializeField] private int _trailDefaultCapacity = 10; // 처음에 준비할 리스트 크기
    [SerializeField] private int _trailMaxPoolSize = 30; // 풀에 보관할 최대 개수, 넘치면 파괴한다.

    private float _fireTimer = 0f;
    private Coroutine _reloadCoroutine = null;

    // 무기마다 남은 총알 수를 따로 가진다. 무기를 바꿔도 다른 무기의 총알은 유지된다.
    private readonly Dictionary<WeaponMode, int> _bulletCounts = new Dictionary<WeaponMode, int>();

    public event Action OnBulletChanged;

    private ObjectPool<PlayerBulletTrail> _trailPool;

    private Animator _animator;
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();

        foreach (WeaponMode mode in Enum.GetValues(typeof(WeaponMode)))
        {
            _bulletCounts[mode] = mode.GetMaxBulletCount();
        }

        _trailPool = new ObjectPool<PlayerBulletTrail>(
            createFunc: CreateTrail,
            actionOnGet: trail => trail.gameObject.SetActive(true),
            actionOnRelease: trail => trail.gameObject.SetActive(false),
            actionOnDestroy: trail => Destroy(trail.gameObject),
            collectionCheck: true,
            defaultCapacity: _trailDefaultCapacity,
            maxSize: _trailMaxPoolSize);

        _animator = GetComponentInChildren<Animator>();
    }

    public int GetBulletCount(WeaponMode mode)
    {
        return _bulletCounts[mode];
    }

    // 풀에 남은 궤적이 없을 때만 호출된다.
    private PlayerBulletTrail CreateTrail()
    {
        PlayerBulletTrail trail = Instantiate(_trailPrefab, _trailParent);
        trail.SetPool(_trailPool);
        return trail;
    }


    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        // 0. 발사 타이머를 지난 시간만큼 줄인다.
        _fireTimer -= Time.deltaTime;

        bool isMouseVisible = CursorManager.IsUiCursor;

        // 1. 마우스 왼쪽 버튼 입력 받기
        WeaponMode mode = _player.WeaponMode;

        if (!isMouseVisible && Input.GetButton("Fire1") && _fireTimer <= 0f && _bulletCounts[mode] > 0)
        {
            _bulletCounts[mode]--;
            OnBulletChanged?.Invoke();
            _fireTimer = _fireInterval;
            Shoot();

            _animator.SetBool("Fire", true);

            // 이 방식은: 이 모노비헤이비어가 등록한 모든 코루틴을 실행 취소
            // StopAllCoroutines();

            if (_reloadCoroutine != null)
            {
                StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
            }
        }
        else
        {
            _animator.SetBool("Fire", false);
        }

        // 2. R키 누르면 총알 재장전 시작
        if (Input.GetKeyDown(KeyCode.R) && _reloadCoroutine == null)
        {
            _reloadCoroutine = StartCoroutine(Reloading_Coroutine(mode));
        }
    }

    // 재장전 도중 무기를 바꿔도 재장전을 시작한 무기의 총알을 채운다.
    private IEnumerator Reloading_Coroutine(WeaponMode mode)
    {
        yield return new WaitForSeconds(_reloadingTime);

        _bulletCounts[mode] = mode.GetMaxBulletCount();
        OnBulletChanged?.Invoke();

        _reloadCoroutine = null;
    }


    private void Shoot()
    {
        StartCoroutine(MuzzleFlash_Coroutine());
        // Ray: 레이저(시작위치, 날아갈 방향)
        // Physics.RayCast: 레이저를 발사
        // RayCastHit: 레이저가 물체와 부딛히면 그 정보를 저장하는 구조체

        // 2. 레이(광선)를 생성하고 발사될 위치와 진행 방향 설정하기
        Ray ray = CameraManager.Instance.GetAimRay();

        // 3. 레이가 부딛힌 대상의 정보를 저장할 변수 생성하기
        RaycastHit hitInfo = new RaycastHit();

        // 4. 레이를 발사하고, 만일 부딛힌 물체가 있다면 이펙트 표시하기
        bool isHit = Physics.Raycast(ray, out hitInfo, _range, CameraManager.Instance.AimMask);

        if (isHit)
        {
            // Debug.Log($"{hitInfo.collider.name}이 총알을 맞았다. 대미지: {_damage}");

            // 맞은 자리에 총알 자국 이펙트 만들기
            ParticleSystem effect = Instantiate(_hitEffectPrefab);
            effect.transform.position = hitInfo.point; // 맞은 자리
            effect.transform.forward = hitInfo.normal; // 수직 위치로 회전
            effect.Play();

            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Value = _damage;
            damageInfo.HitPoint = hitInfo.point;
            damageInfo.HitNormal = hitInfo.normal;

            if (hitInfo.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damageInfo);
            }
        }

        // 5. 총구에서 맞은 지점까지 궤적 그리기 (안 맞았으면 사거리 끝까지)
        Vector3 endPoint = isHit ? hitInfo.point : ray.origin + ray.direction * _range;
        PlayerBulletTrail trail = _trailPool.Get();
        trail.Show(_muzzle.position, endPoint);
    }

    private IEnumerator MuzzleFlash_Coroutine()
    {
        int randomIndex = UnityEngine.Random.Range(0, _muzzleFlashes.Length);
        GameObject flash = _muzzleFlashes[randomIndex];
        flash.SetActive(true);
        yield return new WaitForSeconds(_flashTime);
        flash.SetActive(false);
    }
}