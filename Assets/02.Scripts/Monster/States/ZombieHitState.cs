using UnityEngine;

// 피격 : 공격을 받아 잠깐 멈칫한 뒤 추격한다.
public class ZombieHitState : IZombieState
{
    private const float StunTime = 0.2f; // 멈칫하는 시간

    private readonly Zombie _zombie;
    private float _timer;

    public ZombieHitState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
        // 맞는 중에 또 맞으면 다시 들어오므로 멈칫 시간이 처음부터 다시 시작된다.
        _timer = 0f;
    }

    public void Execute()
    {
        _timer += Time.deltaTime;
        if (_timer >= StunTime)
        {
            _zombie.ChangeState(ZombieState.Trace);
        }
    }

    public void Exit()
    {
    }
}
