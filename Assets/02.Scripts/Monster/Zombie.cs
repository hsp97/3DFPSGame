using System.Collections;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.UIElements;

// FSM(유한 상태 머신) : 유한개의 상태가 있고, 상태마다 행동을 다르게 함으로써 AI를 구현하는 유명한 방법
// - "상태" : 객체가 가질 수 있는 어떤 조건이나 상황
// - "전이" : 조건에 따라 현재 상태에서 다른 상태로 넘어가는 것
// - 그리고 위 방식에 따라 규칙을 세워둔 것을 "머신"

// 좀비의 가실 수 있는 상태
public enum ZombieState
{
    Idle, // 대기 : 제자리에서 플레이어를 기다린다.
    Trace, // 추격 : 플레이어를 쫓아간다.
    Comeback, // 복귀 : 처음자리로 돌아간다.
    Attack, // 공격 : 일정 시간마다 플레이어를 공격한다.
    Hit, // 피격 : 공격을 받아 잠깐 멈칫한다.
    Death, // 죽음 : 쓰러지고 잠시 뒤 사라진다.
    Roaming, // 로밍 : 로밍상태 
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

    private float _attackTimer = 0f;
    private CharacterController _controller;
    private Vector3 _startPosition;
    private GameObject _player;
    private ZombieState _state = ZombieState.Idle;

    public float Health
    {
        get => _health;
    }

    public float MaxHealth
    {
        get => _maxHealth;
    }

    private void Start()
    {
        _health = _maxHealth;
        _controller = GetComponent<CharacterController>();
        _startPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        switch (_state)
        {
            case ZombieState.Idle:
                {
                    // Todo. 대기 : 제자리에서 플레이어를 기다린다.
                    Idle();
                    break;
                }
            case ZombieState.Trace:
                {
                    // Todo. 추격 : 플레이어를 쫓아간다.
                    Trace();
                    break;
                }
            case ZombieState.Comeback:
                {
                    Comeback();
                    break;
                }
            case ZombieState.Attack:
                {
                    Attack();
                    break;
                }
            case ZombieState.Roaming:
                {
                    Roaming();
                    break;
                }
        }
    }

    // 대미지를 받는다.
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_state == ZombieState.Death)
        {
            return;
        }

        _health -= damageInfo.Value;

        StopAllCoroutines();

        if (_health > 0f)
        {
            GameObject bloodSpray = Instantiate(_bloodSprayPrefab, transform);
            bloodSpray.transform.position = damageInfo.HitPoint;
            bloodSpray.transform.forward = damageInfo.HitNormal;

            ChangeState(ZombieState.Hit);
            // 잠깐 멈칫
            StartCoroutine(Hit_Corutine());
        }
        else
        {
            ChangeState(ZombieState.Death);
            StartCoroutine(Death_Corutine());
        }
    }

    private IEnumerator Hit_Corutine()
    {
        yield return new WaitForSeconds(0.2f);
        ChangeState(ZombieState.Trace);
    }

    private IEnumerator Death_Corutine()
    {
        _controller.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void ChangeState(ZombieState nextState)
    {
        Debug.Log($"{name}: {_state} -> {nextState}");
        _state = nextState;
    }


    private void Idle()
    {
        // 대기 : 제자리에서 플레이어를 기다린다.
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // 패트롤 : 특정 포인트를 왔다갔다 한다.
        if (distance > _detectDistance)
        {
            ChangeState(ZombieState.Roaming);
        }

        // 플레이어 가까워지면 Trace 상태로 전이
        if (distance <= _detectDistance)
        {
            ChangeState(ZombieState.Trace);
        }
    }

    public void Roaming()
    {
        Vector3 direction = _romingPoint.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        transform.forward = direction;
        _controller.SimpleMove(direction * _moveSpeed);
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= _detectDistance)
        {
            ChangeState(ZombieState.Trace);
            return;
        }

        if (Vector3.Distance(transform.position, _romingPoint.transform.position) <= 1f)
        {
            ChangeState(ZombieState.Comeback);
        }
    }

    private void Trace()
    {
        // Todo. run 애니메이션 실행
        // 처음 자리에서 너무 멀어지면 돌아간다.
        if (Vector3.Distance(transform.position, _startPosition) > _comebackDistance)
        {
            ChangeState(ZombieState.Comeback);
            return;
        }

        if (Vector3.Distance(transform.position, _player.transform.position) <= _attackRange)
        {
            _attackTimer = 0f;
            ChangeState(ZombieState.Attack);
            return;
        }

        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        transform.forward = direction;
        _controller.SimpleMove(direction * _moveSpeed);
    }

    private void Comeback()
    {
        if (Vector3.Distance(transform.position, _startPosition) <= 1f)
        {
            ChangeState(ZombieState.Idle);
            return;
        }

        Vector3 direction = _startPosition - transform.position;
        direction.y = 0f;
        direction.Normalize();
        transform.forward = direction;
        _controller.SimpleMove(direction * _moveSpeed);
    }

    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > _attackRange)
        {
            ChangeState(ZombieState.Trace);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _attackInterval)
        {
            // Todo. 플레이어 때림
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Value = _attackDamage;
            damageInfo.HitPoint = _player.transform.position;
            damageInfo.HitNormal = (transform.position - _player.transform.position).normalized;
            _player.GetComponent<IDamageable>().TakeDamage(damageInfo);
            _attackTimer = 0;
        }
    }
}