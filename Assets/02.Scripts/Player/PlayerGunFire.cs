using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffectPrefab;

    [Header("스탯")]
    [SerializeField] private float _range = 100f; // 사거리
    [SerializeField] private float _damage = 10f; // 공격력
    [SerializeField] private float _fireInterval = 0.1f; // 발사속도

    private float _fireTimer = 0f;

    private void Update()
    {
        // 0. 발사 타이머를 지난 시간만큼 줄인다.
        _fireTimer -= Time.deltaTime;
        // 1. 마우스 왼쪽 버튼
        // 0~2
        if (Input.GetButton("Fire1") && _fireTimer <= 0f)
        {
            _fireTimer = _fireInterval;
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
            ParticleSystem effect = Instantiate(_hitEffectPrefab);
            effect.transform.position = hitInfo.point; // 맞은 자리
            effect.transform.forward = hitInfo.normal; // 수직 위치로 회전
            effect.Play();
        }
    }
}