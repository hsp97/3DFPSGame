using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private PlayerStats _playerStats;

    private void LateUpdate()
    {
        // 0 ~ 100% -> 0 ~ 1
        _healthSlider.value = _playerStats.Health / _playerStats.MaxHealth;
        _staminaSlider.value = _playerStats.Stamina / _playerStats.MaxStamina;
    }
}