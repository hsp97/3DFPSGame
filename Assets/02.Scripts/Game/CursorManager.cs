using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // Left Alt로 UI를 조작하는 중인지. 이때는 사격과 공격을 하지 않는다.
    public static bool IsUiCursor { get; private set; }

    private void Awake()
    {
        IsUiCursor = false;
        LockCursor();
    }

    private void Start()
    {
        CameraManager.Instance.OnViewChanged += RefreshCursor;
        RefreshCursor();
    }

    private void OnDestroy()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.OnViewChanged -= RefreshCursor;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            UnLockCursor();
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            LockCursor();
        }
    }

    // 시점이 바뀌면 커서 상태도 그 시점에 맞게 다시 정한다.
    private void RefreshCursor()
    {
        if (IsUiCursor)
        {
            return;
        }

        LockCursor();
    }

    public void LockCursor()
    {
        IsUiCursor = false;

        // 쿼터뷰와 탑다운은 마우스 커서로 조준하므로 커서를 보이게 두고 화면 밖으로 나가지 않게만 한다.
        bool isCursorAim = CameraManager.Instance != null && CameraManager.Instance.IsCursorAim;

        Cursor.lockState = isCursorAim ? CursorLockMode.Confined : CursorLockMode.Locked; // 마우스 커서를 화면 가운데에 고정
        Cursor.visible = isCursorAim;
    }

    public void UnLockCursor()
    {
        IsUiCursor = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
