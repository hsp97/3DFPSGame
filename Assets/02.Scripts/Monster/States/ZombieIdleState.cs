// 대기 : 제자리에서 플레이어를 기다린다.
public class ZombieIdleState : IZombieState
{
    private readonly Zombie _zombie;

    public ZombieIdleState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
        // 이동하다 멈출 때 관성으로 미끄러지지 않게 바로 세운다.
        _zombie.StopAgent();
    }

    public void Execute()
    {
        // 플레이어가 가까워지면 추격, 멀리 있으면 로밍한다.
        if (_zombie.DistanceTo(_zombie.PlayerPosition) <= _zombie.DetectDistance)
        {
            _zombie.ChangeState(ZombieState.Trace);
        }
        else
        {
            _zombie.ChangeState(ZombieState.Roaming);
        }
    }

    public void Exit()
    {
    }
}
