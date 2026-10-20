using System;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -20f;
    private float _yVelocity = 0; // 위아래 속도, 매 프레임마다 중력 가속도가 붙는다.
    private bool _isDoubleJump = false;
    private float _stamina = 100f;
    private float _staminaWidth = 100f;
    private float _staminaCurrentWidth = 100f;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpPower = 12f;
    [SerializeField] private float _jumpStamina = 10f;
    [SerializeField] private float _dashStamina = 2f;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private RectTransform _staminaBar;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _staminaBar.pivot = new Vector2(0f, _staminaBar.pivot.y);
        _staminaWidth = _staminaBar.rect.width;
        _staminaCurrentWidth = _staminaWidth;
    }

    private void Update()
    {
        if (_characterController.isGrounded)
        {
            _yVelocity = 0;
            _isDoubleJump = false;
        }

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

        // 4. 점프키를 누르면 점프를 한다.
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            if (_stamina <= _jumpStamina)
            {
                return;
            }

            _yVelocity = _jumpPower;
            _stamina -= _jumpStamina;
            _staminaCurrentWidth -= _jumpStamina;
        }

        if (Input.GetButtonDown("Jump") && !_characterController.isGrounded && !_isDoubleJump)
        {
            if (_stamina <= _jumpStamina)
            {
                return;
            }

            _isDoubleJump = true;
            _yVelocity = _jumpPower;
            _stamina -= _jumpStamina;
            _staminaCurrentWidth -= _jumpStamina;
        }

        // 5. 속도를 구한다.
        if (Input.GetButton("DASH"))
        {
            Dash();
        }
        else
        {
            _moveSpeed = 10f;
        }

        Vector3 velocity = direction * _moveSpeed;
        velocity.y = _yVelocity;
        Move(velocity);
        RegainStamina();

        _staminaBar.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            _staminaCurrentWidth
        );
    }

    // 6. 방향과 속력에 따라 이동한다.
    private void Move(Vector3 velocity)
    {
        // transform.position += direction * _moveSpeed * Time.deltaTime;
        _characterController.Move(velocity * Time.deltaTime);
    }

    private void Dash()
    {
        if (_stamina <= _dashStamina)
        {
            return;
        }

        _moveSpeed = 50f;
        _stamina -= _dashStamina;
        _staminaCurrentWidth -= _dashStamina;
    }

    private void RegainStamina()
    {
        _staminaCurrentWidth = Mathf.MoveTowards(
            _staminaCurrentWidth,
            _staminaWidth,
            10 * Time.deltaTime
        );

        _stamina += 10 * Time.deltaTime;
    }
}