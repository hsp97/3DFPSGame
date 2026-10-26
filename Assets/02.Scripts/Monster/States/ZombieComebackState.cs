// 복귀 : 처음 자리로 돌아간다.
public class ZombieComebackState : IZombieState
{
    private const float ArriveDistance = 1f; // 도착으로 판정하는 거리

    private readonly Zombie _zombie;

    public ZombieComebackState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
        if (_zombie.DistanceTo(_zombie.StartPosition) <= ArriveDistance)
        {
            _zombie.ChangeState(ZombieState.Idle);
            return;
        }

        _zombie.MoveTo(_zombie.StartPosition);
    }

    public void Exit()
    {
    }
}
