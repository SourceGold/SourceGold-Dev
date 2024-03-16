using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public bool doNotDestoryOnLoad;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                _instance = go.AddComponent<T>();
                go.name = typeof(T).Name;
            }
            return _instance;
        }
    }
    protected void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        _instance = this as T;

        if (doNotDestoryOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}