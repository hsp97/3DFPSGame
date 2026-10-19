using UnityEngine;

public class CamearFPS : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 500f;
    private float _accumulationX = 0f; // 좌우 회전 각도
    private float _accumulationY = 0f; // 상하 회전 각도

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        GameLogger.Log($"mouseY : {mouseY}");

        float deltaTime = Time.deltaTime;
        _accumulationX += mouseX * _rotationSpeed * deltaTime;
        _accumulationY += -mouseY * _rotationSpeed * deltaTime;
        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);
        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX, 0f);
    }
}