using UnityEngine;

// 게임 상태 UI : 게임 상태가 바뀔 때 일시정지 팝업과 게임 오버 팝업을 켜고 끈다.
public class UI_GameState : MonoBehaviour
{
    [SerializeField] private GameObject _optionPopup;
    [SerializeField] private GameObject _gameOverPopup;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += Refresh;
        Refresh(GameManager.Instance.State);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= Refresh;
        }
    }

    private void Refresh(GameState state)
    {
        _optionPopup.SetActive(state == GameState.Pause);
        _gameOverPopup.SetActive(state == GameState.GameOver);
    }
}
