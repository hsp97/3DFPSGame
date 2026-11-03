using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    Pause,
    GameOver
}

public class GameManager : SingletonBehaviour<GameManager>
{
    private GameState _state = GameState.Ready;
    public GameState State => _state;

    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private GameObject _optionPopup;
    [SerializeField] private GameObject _gameOverPopup;
    [SerializeField] private Animator _playerAnimator;

    private void Start()
    {
        _optionPopup.SetActive(false);
        _gameOverPopup.SetActive(false);
        StartCoroutine(StartSequence_Coroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_state == GameState.Playing)
            {
                _state = GameState.Pause;
                Time.timeScale = 0f;    // 유니티에서 흐르는 시간
                _optionPopup.SetActive(true);
                CursorManager.Instance.UnLockCursor();
            }
            else if (_state == GameState.Pause)
            {
                Resume();
            }


        }
    }

    private IEnumerator StartSequence_Coroutine()
    {
        _stateText.text = "준비";
        yield return new WaitForSeconds(1.5f);
        _stateText.text = "시작";
        yield return new WaitForSeconds(0.5f);
        _stateText.gameObject.SetActive(false);
        _state = GameState.Playing;
    }

    // 계속하기
    public void Resume()
    {
        _state = GameState.Playing;
        Time.timeScale = 1f;
        _optionPopup.SetActive(false);
        CursorManager.Instance.LockCursor();
    }

    // 다시하기
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 게임종료
    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void GameOver()
    {
        _state = GameState.GameOver;
        _playerAnimator.SetFloat("MoveMotion", 0f);

        // 게임 오버 팝업을 띄우고 버튼을 누를 수 있게 커서를 푼다.
        _gameOverPopup.SetActive(true);
        CursorManager.Instance.UnLockCursor();
    }
}