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
    }
}
