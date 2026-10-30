using System;
using UnityEngine;

// 검 모드에서 공격키(마우스 왼쪽 버튼)를 누르고 있는 동안 Slash 애니메이션을 재생하고 싶다.
public class PlayerSwordAttack : MonoBehaviour
{
    private static readonly int SlashHash = Animator.StringToHash("Slash");

    private Player _player;
    private Animator _animator;

    [SerializeField] private Collider _katanaCollider;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponentInChildren<Animator>();

        // 이벤트가 켜기 전까지는 꺼 둔다.
        _katanaCollider.enabled = false;
    }

    public void ActivateCollider()
    {
        // 카타나 콜라이더 활성화
        _katanaCollider.enabled = true;
    }

    public void UnActivateCollider()
    {
        // 카타나 콜라이더 비활성화
        _katanaCollider.enabled = false;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        bool isSlashing = _player.WeaponMode == WeaponMode.Sword && !Cursor.visible && Input.GetButton("Fire1");
        _animator.SetBool(SlashHash, isSlashing);

        // 공격이 끊겨 UnActivate 이벤트를 못 만나도 콜라이더가 켜진 채 남지 않게 한다.
        if (!isSlashing && _katanaCollider.enabled)
        {
            _katanaCollider.enabled = false;
        }
    }
}
