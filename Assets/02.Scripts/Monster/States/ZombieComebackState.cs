// 복귀 : 처음 자리로 돌아간다.
public class ZombieComebackState : IZombieState
{
    private readonly Zombie _zombie;

    public ZombieComebackState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
        // 시작 위치는 바뀌지 않으므로 들어올 때 한 번만 목적지를 정한다.
        _zombie.MoveTo(_zombie.StartPosition);
    }

    public void Execute()
    {
        if (_zombie.HasArrived)
        {
            _zombie.SetTrigger("MoveToIdle");
            _zombie.ChangeState(ZombieState.Idle);
        }
    }

    public void Exit()
    {
    }
}
