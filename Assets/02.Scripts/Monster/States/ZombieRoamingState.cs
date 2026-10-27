// 로밍 : 로밍 지점으로 이동하고, 도착하면 처음 자리로 돌아간다.
public class ZombieRoamingState : IZombieState
{
    private readonly Zombie _zombie;

    public ZombieRoamingState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
        // 로밍 지점은 움직이지 않으므로 들어올 때 한 번만 목적지를 정한다.
        _zombie.MoveTo(_zombie.RoamingPosition);
    }

    public void Execute()
    {
        if (_zombie.DistanceTo(_zombie.PlayerPosition) <= _zombie.DetectDistance)
        {
            _zombie.ChangeState(ZombieState.Trace);
            return;
        }

        if (_zombie.HasArrived)
        {
            _zombie.ChangeState(ZombieState.Comeback);
        }
    }

    public void Exit()
    {
    }
}
