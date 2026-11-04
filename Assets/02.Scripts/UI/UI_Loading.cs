using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Loading : MonoBehaviour
{
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _progressText;

    private void Start()
    {
        StartCoroutine(Load_Coroutine());
    }

    private IEnumerator Load_Coroutine()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        // 비동기 씬 로드
        // 비동기 진행사항을 확인할 수 있는 객체
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneLoader.NextSceneName);

        // 90%까지 불러온 뒤 씬 전환을 보류한다. 이때 progress는 0.9에서 멈추고 isDone은 false로 남는다.
        operation.allowSceneActivation = false;

        // 씬 전환까지 끝나면 isDone이 true가 된다.
        while (!operation.isDone)
        {
            float value = Mathf.Clamp01(operation.progress / 0.9f);
            _progressSlider.value = value;
            _progressText.text = $"불러오는 중 {Mathf.RoundToInt(value * 100f)}%";

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
