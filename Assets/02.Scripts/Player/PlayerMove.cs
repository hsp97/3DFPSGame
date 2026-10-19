using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;

    private void Update()
    {
        // 1. 키보드 방향을 받는다
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        float v = Input.GetAxisRaw("Vertical");
        // 2. 방향을 구한다.
        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        direction = transform.TransformDirection(direction);
        // 로컬 좌표계 : 한 대상을 중심
        // 월드 좌표계 : 세상을 중심

        // 3. 방향과 속력에 따라 이동한다.
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }
}