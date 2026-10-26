using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] private Button _zoomInButton;
    [SerializeField] private Button _zoomOutButton;
    // Todo. +- 버튼 클릭하면 ZoomIn/ZoomOut 호출

    private void Start()
    {
        _zoomInButton.onClick.AddListener(MinimapCamera.Instance.ZoomIn);
        _zoomOutButton.onClick.AddListener(MinimapCamera.Instance.ZoomOut);
    }
}