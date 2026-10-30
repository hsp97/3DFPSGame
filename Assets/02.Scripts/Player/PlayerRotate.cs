using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 500f;
    private float _accumulationX = 0f;

    // 카메라가 좌우 방향을 맞출 수 있게 현재 바라보는 각도를 공개한다.
    public float Yaw => _accumulationX;

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        // 쿼터뷰와 탑다운은 마우스 커서가 가리키는 쪽을 바라본다.
        if (CameraManager.Instance.IsCursorAim)
        {
            RotateToCursor();
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, _accumulationX, 0f);
    }

    private void RotateToCursor()
    {
        // 커서 위치의 레이가 플레이어 발 높이의 평면과 만나는 지점을 바라본다.
        Ray ray = CameraManager.Instance.GetAimRay();
        Plane ground = new Plane(Vector3.up, transform.position);

        if (!ground.Raycast(ray, out float distance))
        {
            return;
        }

        Vector3 direction = ray.GetPoint(distance) - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        // 다시 FPS로 돌아왔을 때 튀지 않게 누적값도 맞춰 둔다.
        _accumulationX = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, _accumulationX, 0f);
    }
}