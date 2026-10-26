using UnityEngine;
using UnityEngine.Pool;

// G키를 누르면 시선이 바라보는 방향으로 수류탄을 던지고 싶다.
public class PlayerBombFire : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float _throwPower = 15f;

    [Header("오브젝트 풀")]
    [SerializeField] private Transform _bombParent; // 생성된 수류탄을 모아둘 부모 오브젝트(Bombs)
    [SerializeField] private int _defaultCapacity = 10; // 처음에 준비할 리스트 크기
    [SerializeField] private int _maxPoolSize = 30; // 풀에 보관할 최대 개수, 넘치면 파괴한다.

    private ObjectPool<Bomb> _bombPool;

    private void Awake()
    {
        _bombPool = new ObjectPool<Bomb>(
            createFunc: CreateBomb,
            actionOnGet: bomb => bomb.gameObject.SetActive(true),
            actionOnRelease: bomb => bomb.gameObject.SetActive(false),
            actionOnDestroy: bomb => Destroy(bomb.gameObject),
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxPoolSize);
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }
        // 1. G키 입력 받기 (카메라 우클릭과 분리)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Shoot();
        }
    }

    // 풀에 남은 수류탄이 없을 때만 호출된다.
    private Bomb CreateBomb()
    {
        Bomb bomb = Instantiate(_bombPrefab, _bombParent);
        bomb.SetPool(_bombPool);
        return bomb;
    }

    private void Shoot()
    {
        // 2. 풀에서 수류탄을 꺼내 발사 위치로 옮기기
        Bomb bomb = _bombPool.Get();
        bomb.transform.SetPositionAndRotation(_firePoint.position, Quaternion.identity);

        // 3. 재사용된 수류탄은 이전 속도가 남아 있으므로 초기화한 뒤 시선 방향으로 던지기
        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
    }
}
