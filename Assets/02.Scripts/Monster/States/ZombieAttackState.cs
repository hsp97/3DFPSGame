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
        // 붙자마자 첫 공격이 나가도록 타이머를 공격 간격만큼 채운 상태로 시작한다.
        _attackTimer = _zombie.AttackInterval;
        _zombie.SetTrigger("MoveToAttackDelay");
    }

    public void Execute()
    {
        if (_zombie.DistanceTo(_zombie.PlayerPosition) > _zombie.AttackRange)
        {
            _zombie.SetTrigger("AttackMove");
            _zombie.ChangeState(ZombieState.Trace);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _zombie.AttackInterval)
        {
            _zombie.SetTrigger("StartAttack");
            _attackTimer = 0f;
        }
    }

    public void Exit()
    {
    }
}
