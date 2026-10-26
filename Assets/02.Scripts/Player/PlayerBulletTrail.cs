using UnityEngine;
using UnityEngine.Pool;

// 총을 쏠 때 총구에서 맞은 지점까지 잠깐 보이는 총알 궤적
[RequireComponent(typeof(LineRenderer))]
public class PlayerBulletTrail : MonoBehaviour
{
    [SerializeField] private float _duration = 0.05f; // 선이 보이는 시간

    private LineRenderer _line;
    private IObjectPool<PlayerBulletTrail> _pool;
    private float _timer;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 2;
    }

    // 풀에서 생성될 때 한 번 호출된다. 시간이 다 되면 이 풀로 돌아간다.
    public void SetPool(IObjectPool<PlayerBulletTrail> pool)
    {
        _pool = pool;
    }

    public void Show(Vector3 start, Vector3 end)
    {
        _line.SetPosition(0, start);
        _line.SetPosition(1, end);
        _timer = 0f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _duration) return;

        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
