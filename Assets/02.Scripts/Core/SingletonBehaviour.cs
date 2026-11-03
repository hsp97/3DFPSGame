using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        // 씬을 다시 불러오면 새 씬의 인스턴스가 이전 인스턴스를 대체한다.
        // 이전 인스턴스는 사라진 이전 씬의 UI를 참조하고 있으므로 남겨두면 안 된다.
        if (_instance != null && _instance != this)
        {
            Destroy(_instance.gameObject);
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
