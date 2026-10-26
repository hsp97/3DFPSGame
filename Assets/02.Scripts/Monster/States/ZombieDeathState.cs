using UnityEngine;

// 죽음 : 쓰러지고 잠시 뒤 사라진다.
public class ZombieDeathState : IZombieState
{
    private const float DespawnDelay = 0.5f; // 사라지기까지 걸리는 시간

    private readonly Zombie _zombie;
    private float _timer;

    public ZombieDeathState(Zombie zombie)
    {
        _zombie = zombie;
    }

    public void Enter()
    {
        _timer = 0f;
        _zombie.DisableCollision();
    }

    public void Execute()
    {
        _timer += Time.deltaTime;
        if (_timer >= DespawnDelay)
        {
            _zombie.Despawn();
        }
    }

    public void Exit()
    {
    }
}
