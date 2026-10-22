using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;

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
}

public class Zombie : MonoBehaviour
{
    [Header("스탯")]
    [SerializeField] private float _detectDistance = 8; // 플레이어를 발견하는 거리
    [SerializeField] private float _moveSpeed = 4;

    private CharacterController _controller;
    private GameObject _player;
    private ZombieState _state = ZombieState.Idle;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
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
            case ZombieState.Hit:
                {
                    break;
                }
            case ZombieState.Death:
                {
                    break;
                }
        }
    }

    private void Idle()
    {
        // 대기 : 제자리에서 플레이어를 기다린다.

        // 플레이어 가까워지면 Trace 상태로 전이
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= _detectDistance)
        {
            Debug.Log($"{_state} -> TRACE");
            _state = ZombieState.Trace;
        }
    }

    private void Trace()
    {
        // Todo. run 애니메이션 실행
        Vector3 diretion = _player.transform.position - transform.position;
        diretion.y = 0f;
        diretion.Normalize();

        transform.forward = diretion;
        _controller.SimpleMove(diretion * _moveSpeed);
    }

    private void Attack()
    {
        throw new System.NotImplementedException();
    }

    private void Comeback()
    {
        throw new System.NotImplementedException();
    }
}