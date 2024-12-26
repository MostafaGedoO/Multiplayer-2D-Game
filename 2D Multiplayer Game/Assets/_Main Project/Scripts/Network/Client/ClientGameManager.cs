using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager 
{
    private JoinAllocation joinAllocation;

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        AuthenticationState _authenticationState = await AuthenticationHandler.DoAuthentication();

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
        NetworkManager.Singleton.StartClient();
    }
}
