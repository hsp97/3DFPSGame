using System;
using System.Collections.Generic;
using UnityEngine;

// 살아 있는 좀비를 등록해서 세고, 좀비가 죽을 때마다 줄여서 알려준다.
public class ZombieManager : MonoBehaviour
{
    private readonly List<Zombie> _zombies = new List<Zombie>();
    public int ZombieCount => _zombies.Count;

    public event Action<int> OnZombieCountChanged;

    // 모든 Awake가 끝난 뒤에 Start가 실행되므로, UI는 Start에서 개수를 읽으면 안전하다.
    private void Awake()
    {
        Zombie[] zombies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
        foreach (Zombie zombie in zombies)
        {
            Register(zombie);
        }
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

    public void Register(Zombie zombie)
    {
        _zombies.Add(zombie);
        zombie.OnDied += OnZombieKilled;
        OnZombieCountChanged?.Invoke(_zombies.Count);
    }

    private void OnZombieKilled(Zombie zombie)
    {
        zombie.OnDied -= OnZombieKilled;
        _zombies.Remove(zombie);
        OnZombieCountChanged?.Invoke(_zombies.Count);
    }
}
