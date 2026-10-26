using UnityEngine;
using UnityEngine.Pool;

// 일정 간격으로 스폰 지점 중 한 곳에 좀비를 생성한다. 살아 있는 좀비가 최대 수에 도달하면 멈춘다.
public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private Zombie _zombiePrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _roamingPoint;
    [SerializeField] private ZombieManager _zombieManager;

    [Header("스폰")]
    [SerializeField] private float _spawnInterval = 3f; // 생성 간격(초)
    [SerializeField] private int _maxZombieCount = 10; // 동시에 살아 있을 수 있는 최대 좀비 수

    [Header("오브젝트 풀")]
    [SerializeField] private Transform _zombieParent;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxPoolSize = 30;

    private float _spawnTimer = 0f;

    private ObjectPool<Zombie> _zombiePool;
    private void Awake()
    {
        _zombiePool = new ObjectPool<Zombie>(
            createFunc: CreateZombie,
            actionOnGet: zombie => zombie.gameObject.SetActive(true),
            actionOnRelease: zombie => zombie.gameObject.SetActive(false),
            actionOnDestroy: zombie => Destroy(zombie.gameObject),
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxPoolSize
        );
    }

    private Zombie CreateZombie()
    {
        Zombie zombie = Instantiate(_zombiePrefab, _zombieParent);
        zombie.SetPool(_zombiePool);
        return zombie;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer < _spawnInterval) return;

        _spawnTimer = 0f;
        if (_zombieManager.ZombieCount < _maxZombieCount)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        Zombie zombie = _zombiePool.Get();
        zombie.Spawn(spawnPoint.position, spawnPoint.rotation, _roamingPoint);
        _zombieManager.Register(zombie);
    }
}
