using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private PlayerStats _stats;

    [SerializeField] private UI_HitEffect _uiHitEffect;

    [SerializeField] private float _alpha = 1f;
    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_stats.Health <= 0) return;
        _stats.Health -= damageInfo.Value;
        _uiHitEffect.Flash(_alpha);
        Debug.Log($"플레이어 맞음. 대미지 {damageInfo.Value}, 남은체력 {_stats.Health}");
        if (_stats.Health <= 0f)
        {
            Debug.Log("플레이어 죽음");
        }
    }
}