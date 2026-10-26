using System;
using UnityEngine;

// 씬에 있는 좀비 수를 세고, 좀비가 죽을 때마다 줄여서 알려준다.
public class ZombieManager : MonoBehaviour
{
    private int _zombieCount;
    public int ZombieCount => _zombieCount;

    public event Action<int> OnZombieCountChanged;

    private Zombie[] _zombies;

    private void Awake()
    {
        _zombies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
        foreach (Zombie zombie in _zombies)
        {
            zombie.OnDied += OnZombieKilled;
        }

        _zombieCount = _zombies.Length;
    }

    private void OnDestroy()
    {
        foreach (Zombie zombie in _zombies)
        {
            if (zombie != null)
            {
                zombie.OnDied -= OnZombieKilled;
            }
        }
    }

    private void OnZombieKilled()
    {
        _zombieCount--;
        OnZombieCountChanged?.Invoke(_zombieCount);
    }
}
