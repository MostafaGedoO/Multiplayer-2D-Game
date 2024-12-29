
using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager networkManager;
    private const string mainMenuSceneName = "MainMenu";

    public NetworkClient()
    {
        networkManager = NetworkManager.Singleton;

        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == 0 | clientId == networkManager.LocalClientId)
        {
            if (SceneManager.GetActiveScene().name != mainMenuSceneName)
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }

            if(networkManager.IsConnectedClient)
            {
                networkManager.Shutdown();
            }
        }
    }

    public void Dispose()
    {
        if(networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
}
