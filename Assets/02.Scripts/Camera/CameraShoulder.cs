using UnityEngine;

// 플레이어 오른쪽 어깨 너머에서 정면을 바라보는 시점
public class CameraShoulder : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1.5f, 0f); // 머리 높이
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0.7f, 0.3f, -2.2f); // 오른쪽 어깨 뒤
    [SerializeField] private float _rotationSpeed = 500f;

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
            _accumulationY += -Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
            _accumulationY = Mathf.Clamp(_accumulationY, -60f, 60f);
        }

        // 좌우 회전은 플레이어와 같은 값을 써서 플레이어가 바라보는 방향과 카메라 방향이 어긋나지 않게 한다.
        transform.position = _player.position + _targetOffset;
        transform.rotation = Quaternion.Euler(_accumulationY, _playerRotate.Yaw, 0f);
        _cameraTransform.localPosition = _cameraOffset;
        _cameraTransform.localRotation = Quaternion.identity;
    }
}
