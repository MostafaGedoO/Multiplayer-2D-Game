using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientSingletonPrefab;
    [SerializeField] private HostSingleton hostSingletonPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInModeAsync(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInModeAsync(bool isDedicatedServer)
    {
        if(isDedicatedServer) // Dedicated Server
        {

        }
        else //Host or Client
           
        {
            //Create Host Singleton
            HostSingleton _hostSingleton = Instantiate(hostSingletonPrefab);
            _hostSingleton.CreateHost();
         
            //Create Client Singleton
            ClientSingleton _clientSingleton = Instantiate(clientSingletonPrefab);
            bool _authState = await _clientSingleton.CreateClientAsync();

            if(_authState)
            {
                _clientSingleton.ClientGameManager.GoToMainMenu();
            }
        }
    }
}
