using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    public static ClientSingleton Instance
    {
        get
        {
            if(instance != null) return instance;

            instance = FindAnyObjectByType<ClientSingleton>();

            if(instance != null) return instance;

            Debug.LogError(" No Client Singleton In Scene! ");
            return null;
        } 
    }

    private ClientGameManager clientGameManager;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateClientAsync()
    {
        clientGameManager = new ClientGameManager();
        await clientGameManager.InitAsync();
    }
}
