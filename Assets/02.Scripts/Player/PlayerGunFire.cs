using System;
using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    private float _range = 100f;
    private float _damage = 10f;

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼
        // 0~2
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Ray : 레이저(시작위치, 날아갈 방향)
        // Physics.Raycast
        // RaycastHit

        Transform cameraTransform = Camera.main.transform;
        // 2. 레이(광선)을 생성하고 발사될 위치와 진행 방향 설정        
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        // 3. 레이가 부딛힌 대상의 정보를 저장할 변수 생성
        RaycastHit hitInfo = new RaycastHit();

        // 4. 레이를 발사하고, 만일 부딛힌 물체가 있다면 이펙트 표시
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);

        if (isHit)
        {
            Debug.Log(hitInfo.transform.name);
            // Todo. 이펙트 생성
        }
    }
}