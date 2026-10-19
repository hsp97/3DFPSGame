using UnityEngine;

public class CameraQuarter : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cameraTransform;

    // 플레이어의 몸통 근처를 회전 중심으로 사용
    [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1f, 0f);
    // 회전 중심을 기준으로 한 카메라의 상대 위치
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 8f, -6f);

    [SerializeField] private float _rotationSpeed = 100f;
    private float _accumulationX = 0f; // 좌우 회전 각도

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        }

        transform.position = _player.position + _targetOffset;
        transform.rotation = Quaternion.Euler(0f, _accumulationX, 0f);
        _cameraTransform.localPosition = _cameraOffset;
        _cameraTransform.LookAt(transform.position);
    }
}