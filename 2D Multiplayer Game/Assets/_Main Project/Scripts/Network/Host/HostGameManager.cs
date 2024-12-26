using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation allocation;
    public string JoinCode {  get; private set; }

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(20);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }
        
        try
        {
            JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(JoinCode);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }

        RelayServerData serverData = new RelayServerData(allocation,"udp");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
