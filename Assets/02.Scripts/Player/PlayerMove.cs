using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -20f;
    private const int MAXJUMPS = 2;

    private float _yVelocity = 0f; // 위아래 속도, 매 프레임마다 중력 가속도가 붙는다.
    private int _jumpCount = 0;

    private float _lastJumpPressedTime = float.NegativeInfinity; // 마지막 점프 입력 시각
    private float _lastGroundedTime = float.NegativeInfinity; // 마지막 지상 밟은 시각

    [Header("스탯")]
    [SerializeField] private float _sprintCost = 10f;
    [SerializeField] private float _jumpCost = 20f;

    [SerializeField] private float _jumpBufferTime = 0.15f;
    [SerializeField] private float _coyoteTime = 0.15f;

    [Header("컴포넌트")]
    private CharacterController _characterController;
    private PlayerStats _stats;
    private Animator _animator;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        // 점프 누른 순간 기록 
        if (Input.GetButtonDown("Jump"))
        {
            _lastJumpPressedTime = Time.time;
        }

        bool isGrounded = _characterController.isGrounded;
        // 스태미너 회복 => playerStats 에서 관리
        // 1. 키보드 방향을 받는다
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        float v = Input.GetAxisRaw("Vertical");
        // 2. 방향을 구한다.
        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        _animator.SetFloat("MoveMotion", direction.magnitude);
        if (CameraManager.Instance.IsCursorAim)
        {
            // 쿼터뷰와 탑다운은 플레이어가 커서를 보므로, 카메라가 보는 방향을 기준으로 움직인다.
            Vector3 cameraForward = CameraManager.Instance.CameraTransform.forward;
            cameraForward.y = 0f;
            direction = Quaternion.LookRotation(cameraForward.normalized) * direction;
        }
        else
        {
            direction = transform.TransformDirection(direction);
        }
        // 로컬 좌표계 : 한 대상을 중심
        // 월드 좌표계 : 세상을 중심

        // 3. 중력을 누적한다.
        if (!isGrounded)
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }
        else
        {
            _lastGroundedTime = Time.time;

            // 땅에 서있으면 중력/점프 초기화
            _yVelocity = -1f;
            _jumpCount = 0;
        }

        // 코요테 타임
        bool canCoyoteJump = Time.time - _lastGroundedTime <= _coyoteTime;

        // 입력이 점프 버퍼 시간 안에 있는지
        bool hasBufferedJump = Time.time - _lastJumpPressedTime <= _jumpBufferTime;

        // 4. 점프키를 누르기 && 점프 카운트가 MAXJUMPS 이하라면 점프한다.
        if (hasBufferedJump && _jumpCount < MAXJUMPS && _stats.Stamina >= _jumpCost)
        {
            _yVelocity = _stats.JumpPower;

            if (!canCoyoteJump)
            {
                _jumpCount += 2;
            }
            else
            {
                _jumpCount++;
            }

            _stats.Stamina -= _jumpCost;

            _lastJumpPressedTime = float.NegativeInfinity;
            _lastGroundedTime = float.NegativeInfinity;
        }

        // 5. 속도를 구한다.
        float requiredStamina = _sprintCost * Time.deltaTime;
        bool wantSprint = Input.GetButton("Sprint") && _stats.Stamina > requiredStamina;
        float currentSpeed = _stats.MoveSpeed;
        if (wantSprint)
        {
            currentSpeed = _stats.SprintSpeed;
            _stats.Stamina -= requiredStamina;
        }

        Vector3 velocity = direction * currentSpeed;
        velocity.y = _yVelocity;

        // 6. 방향과 속력에 따라 이동한다.
        // transform.position += direction * _moveSpeed * Time.deltaTime;
        _characterController.Move(velocity * Time.deltaTime);
    }
}