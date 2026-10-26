using UnityEngine;

// 공격 : 일정 시간마다 플레이어를 공격한다.
public class ZombieAttackState : IZombieState
{
    private readonly Zombie _zombie;
    private float _attackTimer;

    public ZombieAttackState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
        _attackTimer = 0f;
    }

    public void Execute()
    {
        if (_zombie.DistanceTo(_zombie.PlayerPosition) > _zombie.AttackRange)
        {
            _zombie.ChangeState(ZombieState.Trace);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _zombie.AttackInterval)
        {
            _zombie.AttackPlayer();
            _attackTimer = 0f;
        }
    }

    public void Exit()
    {
    }
}
