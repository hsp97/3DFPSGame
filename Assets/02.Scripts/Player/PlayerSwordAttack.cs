using UnityEngine;

// 검 모드에서 공격키(마우스 왼쪽 버튼)를 누르고 있는 동안 Slash 애니메이션을 재생하고 싶다.
public class PlayerSwordAttack : MonoBehaviour
{
    private static readonly int SlashHash = Animator.StringToHash("Slash");

    private Player _player;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponentInChildren<Animator>();
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
