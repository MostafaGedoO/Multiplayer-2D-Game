using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation joinAllocation;
    NetworkClient networkClient;

    public async Task<bool> InitAsync()
    {
        InitializationOptions _options = new InitializationOptions();
        _options.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());

        await UnityServices.InitializeAsync(_options);
        AuthenticationState _authenticationState = await AuthenticationHandler.DoAuthentication();

        networkClient = new NetworkClient();

        if (_authenticationState == AuthenticationState.Authenticated)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public async Task StartClientAsync(string _code)
    {
        try
        {
            joinAllocation = await Relay.Instance.JoinAllocationAsync(_code);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }

        RelayServerData _relayServerData = new RelayServerData(joinAllocation,"udp");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);
        
        //Setting the Player Data
        UserData _userDate = new UserData { UserName = PlayerPrefs.GetString(PlayerNameHandler.playerNameKey, "Player" + UnityEngine.Random.Range(100, 1000)),
            AuthId = AuthenticationService.Instance.PlayerId };

        //Making the byte[] the we can pass to the networkManager
        string _dataJson = JsonUtility.ToJson(_userDate);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(_dataJson);


        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        networkClient.Disconnect();
    }
    public void Dispose()
    {
        networkClient?.Dispose();
    }

}
