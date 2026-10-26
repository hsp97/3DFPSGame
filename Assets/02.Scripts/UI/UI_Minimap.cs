using UnityEngine;
using UnityEngine.UI;

// 미니맵 UI : +- 버튼을 누르면 미니맵 카메라를 확대/축소한다.
public class UI_Minimap : MonoBehaviour
{
    [SerializeField] private Button _zoomInButton;
    [SerializeField] private Button _zoomOutButton;

    private void Start()
    {
        _zoomInButton.onClick.AddListener(MinimapCamera.Instance.ZoomIn);
        _zoomOutButton.onClick.AddListener(MinimapCamera.Instance.ZoomOut);
    }

    private void OnDestroy()
    {
        _zoomInButton.onClick.RemoveAllListeners();
        _zoomOutButton.onClick.RemoveAllListeners();
    }
}
