using UnityEngine;

// 조준점을 현재 시점의 조준 위치에 맞춘다.
// 쿼터뷰와 탑다운은 마우스 커서가 가리키는 곳, 나머지 시점은 화면 가운데를 조준한다.
public class UI_Crosshair : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
    }

    private void LateUpdate()
    {
        bool followCursor = CameraManager.Instance.IsCursorAim && !CursorManager.IsUiCursor;

        // Screen Space - Overlay 캔버스는 화면 좌표를 그대로 위치로 쓸 수 있다.
        _rectTransform.position = followCursor
            ? (Vector2)Input.mousePosition
            : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }
}
