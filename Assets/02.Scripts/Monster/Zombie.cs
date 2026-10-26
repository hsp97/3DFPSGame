using System;
using System.Collections.Generic;
using UnityEngine;

// FSM(유한 상태 머신) : 유한개의 상태가 있고, 상태마다 행동을 다르게 함으로써 AI를 구현하는 유명한 방법
// - "상태" : 객체가 가질 수 있는 어떤 조건이나 상황
// - "전이" : 조건에 따라 현재 상태에서 다른 상태로 넘어가는 것
// - 그리고 위 방식에 따라 규칙을 세워둔 것을 "머신"
// 상태별 행동은 States 폴더의 상태 클래스가 담당하고, Zombie는 데이터와 공용 기능, 상태 전환만 맡는다.

// 좀비의 가실 수 있는 상태
public enum ZombieState
{
    Idle, // 대기 : 제자리에서 플레이어를 기다린다.
    Trace, // 추격 : 플레이어를 쫓아간다.
    Comeback, // 복귀 : 처음자리로 돌아간다.
    Attack, // 공격 : 일정 시간마다 플레이어를 공격한다.
    Hit, // 피격 : 공격을 받아 잠깐 멈칫한다.
    Death, // 죽음 : 쓰러지고 잠시 뒤 사라진다.
    Roaming, // 로밍 : 로밍 지점으로 이동한다.
}

public class Zombie : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float _detectDistance = 8; // 플레이어를 발견하는 거리
    [SerializeField] private float _comebackDistance = 15;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackRange = 1.5f; // 공격 범위
    [SerializeField] private float _attackInterval = 1.2f; // 공격 간격
    [SerializeField] private float _moveSpeed = 4;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _health = 100f;
    [SerializeField] private GameObject _romingPoint;


    [Header("이펙트")]
    [SerializeField] private GameObject _bloodSprayPrefab;

    private CharacterController _controller;
    private Vector3 _startPosition;
    private GameObject _player;

    private ZombieState _state = ZombieState.Idle;
    private Dictionary<ZombieState, IZombieState> _states;
    private IZombieState _currentState;

    public event Action OnDamaged;
    public event Action<Zombie> OnDied;

    public float Health
    {
        get => _health;
    }

    public float MaxHealth
    {
        get => _maxHealth;
    }

    public float DetectDistance => _detectDistance;
    public float ComebackDistance => _comebackDistance;
    public float AttackRange => _attackRange;
    public float AttackInterval => _attackInterval;

    public Vector3 StartPosition => _startPosition;
    public Vector3 PlayerPosition => _player.transform.position;
    public Vector3 RoamingPosition => _romingPoint.transform.position;

    private void Awake()
    {
        _states = new Dictionary<ZombieState, IZombieState>
        {
            { ZombieState.Idle, new ZombieIdleState(this) },
            { ZombieState.Roaming, new ZombieRoamingState(this) },
            { ZombieState.Trace, new ZombieTraceState(this) },
            { ZombieState.Comeback, new ZombieComebackState(this) },
            { ZombieState.Attack, new ZombieAttackState(this) },
            { ZombieState.Hit, new ZombieHitState(this) },
            { ZombieState.Death, new ZombieDeathState(this) },
        };
    }

    private void Start()
    {
        _health = _maxHealth;
        _controller = GetComponent<CharacterController>();
        _startPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player");

        _currentState = _states[_state];
        _currentState.Enter();
    }

    // 스포너가 생성 직후 호출한다. 프리팹은 씬의 로밍 지점을 참조할 수 없기 때문이다.
    public void SetRoamingPoint(GameObject roamingPoint)
    {
        _romingPoint = roamingPoint;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        _currentState.Execute();
    }

    // 대미지를 받는다.
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_state == ZombieState.Death)
        {
            return;
        }

        _health -= damageInfo.Value;

        OnDamaged?.Invoke();

        if (_health > 0f)
        {
            GameObject bloodSpray = Instantiate(_bloodSprayPrefab, transform);
            bloodSpray.transform.position = damageInfo.HitPoint;
            bloodSpray.transform.forward = damageInfo.HitNormal;

            // 잠깐 멈칫
            ChangeState(ZombieState.Hit);
        }
        else
        {
            ChangeState(ZombieState.Death);
            OnDied?.Invoke(this);
        }
    }

    public void ChangeState(ZombieState nextState)
    {
        Debug.Log($"{name}: {_state} -> {nextState}");

        _currentState?.Exit();
        _state = nextState;
        _currentState = _states[nextState];
        _currentState.Enter();
    }

    // 아래는 상태들이 같이 쓰는 기능이다.

    public float DistanceTo(Vector3 position)
    {
        return Vector3.Distance(transform.position, position);
    }

    // 목표 지점을 바라보며 수평으로 이동한다.
    public void MoveTo(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        direction.Normalize();

        transform.forward = direction;
        _controller.SimpleMove(direction * _moveSpeed);
    }

    public void AttackPlayer()
    {
        DamageInfo damageInfo = new DamageInfo();
        damageInfo.Value = _attackDamage;
        damageInfo.HitPoint = PlayerPosition;
        damageInfo.HitNormal = (transform.position - PlayerPosition).normalized;
        _player.GetComponent<IDamageable>().TakeDamage(damageInfo);
    }

    // 죽은 뒤 더 이상 충돌하거나 밀리지 않게 한다.
    public void DisableCollision()
    {
        _controller.enabled = false;
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
