using UnityEngine;

public abstract class SingletonManagerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[SingletonManagerBase] Duplicate instance of {typeof(T)} found, destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
