using UnityEngine;

// 죽음 : 쓰러지고 잠시 뒤 사라진다.
public class ZombieDeathState : IZombieState
{
    private const float DespawnDelay = 2f; // 사라지기까지 걸리는 시간

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
        _zombie.SetTrigger("Death");
        if (_timer >= DespawnDelay)
        {
            _zombie.Despawn();
        }
    }

    public void Exit()
    {
    }
}
