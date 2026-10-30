using UnityEngine;

public class PlayerZoom : MonoBehaviour
{
    [SerializeField] private GameObject _sniperZoom;
    [SerializeField] private float _normalPov = 60f;
    [SerializeField] private float _sniperPov = 15f;

    private Player _player;
    private void Start()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        // 조준경 화면은 1인칭에서만 쓴다. 쿼터뷰는 오른쪽 버튼이 카메라 회전이기도 하다.
        bool isZooming = _player.WeaponMode == WeaponMode.Sniper && Input.GetMouseButton(1)
            && CameraManager.Instance.Mode == CameraViewMode.FPS;
        _sniperZoom.SetActive(isZooming);
        Camera.main.fieldOfView = isZooming ? _sniperPov : _normalPov;
    }
}
