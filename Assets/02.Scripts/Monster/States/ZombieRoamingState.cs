// 로밍 : 로밍 지점으로 이동하고, 도착하면 처음 자리로 돌아간다.
public class ZombieRoamingState : IZombieState
{
    private const float ArriveDistance = 1f; // 도착으로 판정하는 거리

    private readonly Zombie _zombie;

    public ZombieRoamingState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
        _zombie.MoveTo(_zombie.RoamingPosition);

        if (_zombie.DistanceTo(_zombie.PlayerPosition) <= _zombie.DetectDistance)
        {
            _zombie.ChangeState(ZombieState.Trace);
            return;
        }

        if (_zombie.DistanceTo(_zombie.RoamingPosition) <= ArriveDistance)
        {
            _zombie.ChangeState(ZombieState.Comeback);
        }
    }

    public void Exit()
    {
    }
}
