// 추격 : 플레이어를 쫓아간다.
public class ZombieTraceState : IZombieState
{
    private readonly Zombie _zombie;

    public ZombieTraceState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
        // Todo. run 애니메이션 실행
    }

    public void Execute()
    {
        // 처음 자리에서 너무 멀어지면 돌아간다.
        if (_zombie.DistanceTo(_zombie.StartPosition) > _zombie.ComebackDistance)
        {
            _zombie.ChangeState(ZombieState.Comeback);
            return;
        }

        if (_zombie.DistanceTo(_zombie.PlayerPosition) <= _zombie.AttackRange)
        {
            _zombie.ChangeState(ZombieState.Attack);
            return;
        }

        _zombie.MoveTo(_zombie.PlayerPosition);
    }

    public void Exit()
    {
    }
}
