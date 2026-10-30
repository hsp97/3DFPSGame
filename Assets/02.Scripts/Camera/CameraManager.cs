using System;
using UnityEngine;

public enum CameraViewMode
{
    FPS,
    TPS,
    Quarter,
    Shoulder,
    TopDown,
}

// [ ] 키로 카메라 시점을 전환한다.
// Main Camera는 하나만 두고, 시점마다 카메라가 붙는 부모와 켜져 있는 스크립트만 바꾼다.
public class CameraManager : MonoBehaviour
{
    [Serializable]
    private class CameraView
    {
        [SerializeField] private CameraViewMode _mode;
        [SerializeField] private Transform _anchor; // 이 시점일 때 카메라가 붙는 부모
        [SerializeField] private MonoBehaviour[] _behaviours; // 이 시점일 때만 켜지는 스크립트

        public CameraViewMode Mode => _mode;

        public void Apply(bool isCurrent, Transform cameraTransform)
        {
            foreach (MonoBehaviour behaviour in _behaviours)
            {
                behaviour.enabled = isCurrent;
            }

            if (!isCurrent)
            {
                return;
            }

            cameraTransform.SetParent(_anchor, false);
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = Quaternion.identity;
        }
    }

    public static CameraManager Instance { get; private set; }

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CameraViewMode _startMode = CameraViewMode.FPS;
    [SerializeField] private CameraView[] _views;

    private Camera _camera;
    private CameraViewMode _mode;

    public CameraViewMode Mode => _mode;
    public Transform CameraTransform => _cameraTransform;

    // 쿼터뷰와 탑다운은 화면 가운데가 아니라 마우스 커서가 가리키는 곳을 조준한다.
    public bool IsCursorAim => _mode == CameraViewMode.Quarter || _mode == CameraViewMode.TopDown;

    // 조준 레이가 플레이어 자신의 몸에 맞지 않게 제외할 레이어
    public int AimMask => ~LayerMask.GetMask("Player");

    public event Action OnViewChanged;

    private void Awake()
    {
        Instance = this;
        _camera = _cameraTransform.GetComponent<Camera>();
        _mode = _startMode;
    }

    private void Start()
    {
        ApplyMode();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            Cycle(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            Cycle(-1);
        }
    }

    private void Cycle(int step)
    {
        int count = Enum.GetValues(typeof(CameraViewMode)).Length;
        int next = ((int)_mode + step + count) % count;
        SetMode((CameraViewMode)next);
    }

    public void SetMode(CameraViewMode mode)
    {
        if (_mode == mode)
        {
            return;
        }

        _mode = mode;
        ApplyMode();
    }

    private void ApplyMode()
    {
        foreach (CameraView view in _views)
        {
            view.Apply(view.Mode == _mode, _cameraTransform);
        }

        OnViewChanged?.Invoke();
    }

    // 사격과 투척이 향하는 레이. 커서 방식은 마우스 위치, 나머지는 화면 가운데 방향이다.
    public Ray GetAimRay()
    {
        if (IsCursorAim)
        {
            return _camera.ScreenPointToRay(Input.mousePosition);
        }

        return new Ray(_cameraTransform.position, _cameraTransform.forward);
    }

    // origin에서 조준 지점까지의 방향. 커서 방식은 커서가 가리키는 지점을 향한다.
    public Vector3 GetAimDirection(Vector3 origin)
    {
        Ray ray = GetAimRay();

        if (IsCursorAim && Physics.Raycast(ray, out RaycastHit hitInfo, 100f, AimMask))
        {
            return (hitInfo.point - origin).normalized;
        }

        return ray.direction;
    }
}
