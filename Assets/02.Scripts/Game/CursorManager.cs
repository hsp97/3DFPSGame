using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Awake()
    {
        LockCursor();
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

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 화면 가운데에 고정
        Cursor.visible = false;
    }

    public void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}