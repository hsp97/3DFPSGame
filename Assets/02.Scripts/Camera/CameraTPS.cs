using UnityEngine;

public class CameraTPS : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 2f, -4f);
    [SerializeField] private float _rotationSpeed = 100f;

    private PlayerRotate _playerRotate;
    private float _accumulationY = 0f; // 상하 회전 각도

    private void Awake()
    {
        _playerRotate = _player.GetComponent<PlayerRotate>();
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.State == GameState.Playing)
        {
            float mouseY = Input.GetAxis("Mouse Y");
            _accumulationY += -mouseY * _rotationSpeed * Time.deltaTime;
            _accumulationY = Mathf.Clamp(_accumulationY, -90f, 50f);
        }

        // 좌우 회전은 플레이어와 같은 값을 써서 플레이어가 바라보는 방향과 카메라 방향이 어긋나지 않게 한다.
        transform.position = _player.position + _targetOffset;
        transform.eulerAngles = new Vector3(_accumulationY, _playerRotate.Yaw, 0f);
        _cameraTransform.localPosition = _cameraOffset;
        _cameraTransform.LookAt(transform.position);
    }
}