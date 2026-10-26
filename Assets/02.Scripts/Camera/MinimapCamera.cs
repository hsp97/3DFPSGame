using System;
using UnityEngine;
using UnityEngine.Events;

// 미니맵 카메라 : 플레이어 머리 위에서 아래로 내려다보며 따라다닌다.
public class MinimapCamera : MonoBehaviour
{
    private static MinimapCamera _instance;
    public static MinimapCamera Instance
    {
        get => _instance;
    }

    [SerializeField] private Transform _target;
    [SerializeField] private float _height = 8f;

    private Camera _camera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ZoomIn();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ZoomOut();
        }
    }

    private void LateUpdate()
    {
        // 1. 대상의 머리위로 위치를 옮긴다.
        transform.position = _target.position + (Vector3.up * _height);

        // 2. 아래를 보되, 대상이 쳐다보는 방향으로 카메라도 회전한다.
        transform.eulerAngles = new Vector3(90f, _target.eulerAngles.y, 0f);
    }

    // Todo. +- 버튼 누르면 ZoomIn/ZoomOut 기능 구현
    public void ZoomIn()
    {
        _camera.orthographicSize -= 1;
    }

    public void ZoomOut()
    {
        _camera.orthographicSize += 1;
    }
}