using TMPro;
using UnityEngine;

// 남은 좀비 수 UI : 좀비 수가 변할 때만 텍스트를 갱신한다.
public class UI_EnemyCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private ZombieManager _zombieManager;

    private void Start()
    {
        _zombieManager.OnZombieCountChanged += Refresh;
        Refresh(_zombieManager.ZombieCount);
    }

    private void OnDestroy()
    {
        if (_zombieManager != null)
        {
            _zombieManager.OnZombieCountChanged -= Refresh;
        }
    }

    private void Refresh(int count)
    {
        _text.text = $"남은 적 수 : {count}";
    }
}
