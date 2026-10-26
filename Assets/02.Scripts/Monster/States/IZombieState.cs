// 좀비 상태가 지켜야 하는 약속
public interface IZombieState
{
    // 상태에 들어올 때 한 번 호출된다.
    void Enter();

    // 상태가 유지되는 동안 매 프레임 호출된다.
    void Execute();

    // 상태에서 나갈 때 한 번 호출된다.
    void Exit();
}
