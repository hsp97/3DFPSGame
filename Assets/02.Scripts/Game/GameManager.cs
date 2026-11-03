using System;
using System.Collections;
using TMPro;
using UnityEngine;

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
    [SerializeField] private Animator _playerAnimator;

    private void Start()
    {
        _optionPopup.SetActive(false);
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
                _state = GameState.Playing;
                Time.timeScale = 1f;
                _optionPopup.SetActive(false);
                CursorManager.Instance.LockCursor();
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

    public void GameOver()
    {
        _state = GameState.GameOver;
        _stateText.gameObject.SetActive(true);
        _stateText.text = "게임오버";
        _stateText.color = Color.red;
        _playerAnimator.SetFloat("MoveMotion", 0f);
    }
}