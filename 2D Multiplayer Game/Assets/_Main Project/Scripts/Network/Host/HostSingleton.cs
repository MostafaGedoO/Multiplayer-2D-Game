using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindAnyObjectByType<HostSingleton>();

            if (instance != null) return instance;

            Debug.LogError(" No Host Singleton In Scene! ");
            return null;
        }
    }

    public HostGameManager HostGameManager { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        HostGameManager = new HostGameManager();
    }
}
