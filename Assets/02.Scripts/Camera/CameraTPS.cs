using UnityEngine;

public class CameraTPS : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 2f, -4f);
    [SerializeField] private float _rotationSpeed = 100f;

    private float _accumulationX = 0f; // 좌우 회전 각도
    private float _accumulationY = 0f; // 상하 회전 각도

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        _accumulationY += -mouseY * _rotationSpeed * Time.deltaTime;
        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);

        transform.position = _player.position + _targetOffset;
        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX, 0f);
        _cameraTransform.localPosition = _cameraOffset;
        _cameraTransform.LookAt(transform.position);
    }
}