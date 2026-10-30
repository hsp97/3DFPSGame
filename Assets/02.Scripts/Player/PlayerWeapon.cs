using System;
using TMPro;
using UnityEngine;

// 현재 무기와 무기별 남은 총알 수, 수류탄 수를 UI에 보여준다. 값이 바뀔 때만 갱신한다.
public class PlayerWeapon : MonoBehaviour
{
    [Serializable]
    private class WeaponSlot
    {
        [SerializeField] private WeaponMode _mode;
        [SerializeField] private GameObject _root; // 이 무기의 UI 묶음. 현재 무기일 때만 보인다.
        [SerializeField] private TextMeshProUGUI _remainText; // 검처럼 총알이 없는 무기는 비워 둔다.
        [SerializeField] private TextMeshProUGUI _granadeText; // 수류탄을 쓰지 않는 무기는 비워 둔다.

        public WeaponMode Mode => _mode;

        public void Refresh(bool isCurrent, int bulletCount, int grenadeCount)
        {
            _root.SetActive(isCurrent);

            if (_remainText != null)
            {
                _remainText.text = $"{bulletCount} / {_mode.GetMaxBulletCount()}";
            }

            if (_granadeText != null)
            {
                _granadeText.text = $"폭탄 {grenadeCount}";
            }
        }
    }

    [SerializeField] private Player _player;
    [SerializeField] private PlayerGunFire _gunFire;
    [SerializeField] private PlayerBombFire _bombFire;
    [SerializeField] private WeaponSlot[] _slots;

    private void Start()
    {
        _player.OnWeaponModeChanged += Refresh;
        _gunFire.OnBulletChanged += Refresh;
        _bombFire.OnGrenadeChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnWeaponModeChanged -= Refresh;
        }

        if (_gunFire != null)
        {
            _gunFire.OnBulletChanged -= Refresh;
        }

        if (_bombFire != null)
        {
            _bombFire.OnGrenadeChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        foreach (WeaponSlot slot in _slots)
        {
            slot.Refresh(slot.Mode == _player.WeaponMode, _gunFire.GetBulletCount(slot.Mode), _bombFire.GrenadeCount);
        }
    }
}
