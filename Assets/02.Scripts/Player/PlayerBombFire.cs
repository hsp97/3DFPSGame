using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

// G키를 누르고 있다 떼면 시선이 바라보는 방향으로 수류탄을 던지고 싶다. 오래 누를수록 더 멀리 던진다.
public class PlayerBombFire : MonoBehaviour
{
    private static readonly int ThrowHash = Animator.StringToHash("Throw");

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Bomb _bombPrefab;

    [Header("투척")]
    [FormerlySerializedAs("_throwPower")]
    [SerializeField] private float _minThrowPower = 15f; // 바로 뗐을 때의 힘
    [SerializeField] private float _maxThrowPower = 30f; // 최대로 모았을 때의 힘
    [SerializeField] private float _maxChargeTime = 1.5f; // 최대 힘에 도달하는 데 걸리는 시간(초)
    [SerializeField] private float _releaseDelay = 0.5f; // 투척 모션 시작 후 손에서 수류탄이 떠나는 시점(초)

    [Header("오브젝트 풀")]
    [SerializeField] private Transform _bombParent; // 생성된 수류탄을 모아둘 부모 오브젝트(Bombs)
    [SerializeField] private int _defaultCapacity = 10; // 처음에 준비할 리스트 크기
    [SerializeField] private int _maxPoolSize = 30; // 풀에 보관할 최대 개수, 넘치면 파괴한다.

    private ObjectPool<Bomb> _bombPool;
    private Animator _animator;

    private float _chargeTime = 0f;
    private bool _isCharging = false;
    private bool _isThrowing = false; // 투척 모션이 진행 중이면 새로 던질 수 없다.

    private Player _player;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        _bombPool = new ObjectPool<Bomb>(
            createFunc: CreateBomb,
            actionOnGet: bomb => bomb.gameObject.SetActive(true),
            actionOnRelease: bomb => bomb.gameObject.SetActive(false),
            actionOnDestroy: bomb => Destroy(bomb.gameObject),
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxPoolSize);
    }

    private void Start()
    {
        _player.GetComponent<Player>();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }
        if (_isThrowing)
        {
            return;
        }

        // 1. G키를 누르면 힘 모으기를 시작한다. (카메라 우클릭과 분리)
        if (Input.GetKeyDown(KeyCode.G))
        {
            _isCharging = true;
            _chargeTime = 0f;
        }

        if (!_isCharging)
        {
            return;
        }

        _chargeTime = Mathf.Min(_chargeTime + Time.deltaTime, _maxChargeTime);

        // 2. G키를 떼면 모은 시간에 비례한 힘으로 던진다.
        // GetKeyUp 대신 GetKey로 확인해서, 게임 상태가 바뀌는 동안 뗀 입력도 놓치지 않는다.
        if (!Input.GetKey(KeyCode.G) && _player.WeaponMode == WeaponMode.Normal)
        {
            _isCharging = false;
            float power = Mathf.Lerp(_minThrowPower, _maxThrowPower, _chargeTime / _maxChargeTime);
            StartCoroutine(Throw_Coroutine(power));
        }
    }

    private IEnumerator Throw_Coroutine(float power)
    {
        _isThrowing = true;
        _animator.SetTrigger(ThrowHash);

        // 모션에서 손이 수류탄을 놓는 시점까지 기다렸다가 던진다.
        yield return new WaitForSeconds(_releaseDelay);

        Shoot(power);
        _isThrowing = false;
    }

    // 풀에 남은 수류탄이 없을 때만 호출된다.
    private Bomb CreateBomb()
    {
        Bomb bomb = Instantiate(_bombPrefab, _bombParent);
        bomb.SetPool(_bombPool);
        return bomb;
    }

    private void Shoot(float power)
    {
        // 3. 풀에서 수류탄을 꺼내 발사 위치로 옮기기
        Bomb bomb = _bombPool.Get();
        bomb.transform.SetPositionAndRotation(_firePoint.position, Quaternion.identity);

        // 4. 재사용된 수류탄은 이전 속도가 남아 있으므로 초기화한 뒤 시선 방향으로 던지기
        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.AddForce(Camera.main.transform.forward * power, ForceMode.Impulse);
    }
}
