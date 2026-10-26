using UnityEngine;

// 미니맵 카메라 : 플레이어 머리 위에서 아래로 내려다보며 따라다닌다.
public class MinimapCamera : MonoBehaviour
{
    private static MinimapCamera _instance;
    public static MinimapCamera Instance
    {
        get => _instance;
    }

    [SerializeField] private Transform _target;
    [SerializeField] private float _height = 8f;

    [Header("줌")]
    [SerializeField] private float _zoomStep = 1f; // 한 번 줌할 때 바뀌는 크기
    [SerializeField] private float _minSize = 3f; // 최대로 확대했을 때 크기
    [SerializeField] private float _maxSize = 20f; // 최대로 축소했을 때 크기

    private Camera _camera;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _camera = GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ZoomIn();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ZoomOut();
        }
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // 1. 대상의 머리위로 위치를 옮긴다.
        transform.position = _target.position + (Vector3.up * _height);

        // 2. 아래를 보되, 대상이 쳐다보는 방향으로 카메라도 회전한다.
        transform.eulerAngles = new Vector3(90f, _target.eulerAngles.y, 0f);
    }

    public void ZoomIn()
    {
        SetZoom(_camera.orthographicSize - _zoomStep);
    }

    public void ZoomOut()
    {
        SetZoom(_camera.orthographicSize + _zoomStep);
    }

    // 미니맵 크기를 최소~최대 범위 안으로 제한해서 적용한다.
    private void SetZoom(float size)
    {
        _camera.orthographicSize = Mathf.Clamp(size, _minSize, _maxSize);
    }
}
