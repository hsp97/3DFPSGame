using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -20f;
    private const int MAXJUMPS = 2;
    private const float MAXSTAMINA = 100f;

    private float _yVelocity = 0f; // 위아래 속도, 매 프레임마다 중력 가속도가 붙는다.
    private int _jumpCount = 0;
    private float _stamina = 0f;

    [Header("스탯")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _sprintSpeed = 20f;
    [SerializeField] private float _jumpPower = 12f;
    [SerializeField] private float _staminaRecovery = 15f;
    [SerializeField] private float _sprintCost = 10f;
    [SerializeField] private float _jumpCost = 20f;

    [Header("컴포넌트")]
    [SerializeField] private CharacterController _characterController;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stamina = MAXSTAMINA;
    }

    private void Update()
    {
        // 스태미너 회복
        _stamina = Mathf.Min(MAXSTAMINA, _stamina + _staminaRecovery * Time.deltaTime);
        // 1. 키보드 방향을 받는다
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        float v = Input.GetAxisRaw("Vertical");
        // 2. 방향을 구한다.
        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        direction = transform.TransformDirection(direction);
        // 로컬 좌표계 : 한 대상을 중심
        // 월드 좌표계 : 세상을 중심

        // 3. 중력을 누적한다.
        if (!_characterController.isGrounded)
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }
        else
        {
            // 땅에 서있으면 중력/점프 초기화
            _yVelocity = -1f;
            _jumpCount = 0;
        }

        // 4. 점프키를 누르기 && 점프 카운트가 MAXJUMPS 이하라면 점프한다.
        if (Input.GetButtonDown("Jump") && _jumpCount < MAXJUMPS && _stamina >= _jumpCost)
        {
            _yVelocity = _jumpPower;
            _jumpCount++;
            _stamina -= _jumpCount;
        }

        // 5. 속도를 구한다.
        float requiredStamina = _sprintCost * Time.deltaTime;
        bool wantSprint = Input.GetButton("Sprint") && _stamina > requiredStamina;
        float currentSpeed = _moveSpeed;
        if (wantSprint)
        {
            currentSpeed = _sprintSpeed;
            _stamina -= requiredStamina;
        }

        Vector3 velocity = direction * currentSpeed;
        velocity.y = _yVelocity;

        // 6. 방향과 속력에 따라 이동한다.
        // transform.position += direction * _moveSpeed * Time.deltaTime;
        _characterController.Move(velocity * Time.deltaTime);
    }
}