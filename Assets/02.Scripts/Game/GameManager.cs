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

    // 게임 상태가 바뀔 때 알린다. 팝업 등 상태에 따른 UI는 UI_GameState가 처리한다.
    public event Action<GameState> OnStateChanged;

    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private Animator _playerAnimator;

    private void Start()
    {
        StartCoroutine(StartSequence_Coroutine());
    }

    private void ChangeState(GameState state)
    {
        _state = state;
        OnStateChanged?.Invoke(_state);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_state == GameState.Playing)
            {
                ChangeState(GameState.Pause);
                Time.timeScale = 0f;    // 유니티에서 흐르는 시간
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
        ChangeState(GameState.Playing);
    }

    // 계속하기
    public void Resume()
    {
        ChangeState(GameState.Playing);
        Time.timeScale = 1f;
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
        ChangeState(GameState.GameOver);
        _playerAnimator.SetFloat("MoveMotion", 0f);
    }
}