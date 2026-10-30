using UnityEngine;

// 플레이어 바로 위에서 아래를 내려다보는 시점
public class CameraTopDown : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private float _distance = 15f;
    [SerializeField] private float _pitch = 80f; // 90도면 이동 방향의 기준을 잡을 수 없어 살짝 기울인다.

    private void LateUpdate()
    {
        transform.position = _player.position + _targetOffset;
        transform.rotation = Quaternion.Euler(_pitch, 0f, 0f);
        _cameraTransform.localPosition = new Vector3(0f, 0f, -_distance);
        _cameraTransform.localRotation = Quaternion.identity;
    }
}
