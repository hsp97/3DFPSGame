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

    [Header("갱신")]
    [SerializeField] private float _renderInterval = 1f / 15f; // 미니맵을 다시 그리는 간격(초)

    private Camera _camera;
    private float _renderTimer;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _camera = GetComponent<Camera>();

        // 매 프레임 자동으로 그리지 않고, LateUpdate에서 정해진 간격마다 직접 그린다.
        _camera.enabled = false;
        _renderTimer = _renderInterval; // 첫 프레임에 바로 한 번 그린다.
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

        // 3. 정해진 간격이 지났을 때만 미니맵을 다시 그린다.
        _renderTimer += Time.unscaledDeltaTime;
        if (_renderTimer < _renderInterval) return;

        _renderTimer = 0f;
        _camera.Render();
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
        _renderTimer = _renderInterval; // 줌을 바꾸면 다음 LateUpdate에서 바로 다시 그린다.
    }
}
