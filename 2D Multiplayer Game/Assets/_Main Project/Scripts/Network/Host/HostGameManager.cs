using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation allocation;

    private int maxPlayers = 20;
    public string JoinCode {  get; private set; }

    private string lobbyId;


    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(maxPlayers);
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

        try
        {
            CreateLobbyOptions _lobbyOptions = new CreateLobbyOptions();
            _lobbyOptions.IsPrivate = false;
            _lobbyOptions.Data = new Dictionary<string, DataObject> {{"JoinCode",new DataObject(visibility: DataObject.VisibilityOptions.Member,value: JoinCode)}};

            Lobby _lobby = await Lobbies.Instance.CreateLobbyAsync("Gedo's Room", maxPlayers, _lobbyOptions);
            lobbyId = _lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    private IEnumerator HeartBeatLobby(int _waitTime)
    {
        WaitForSecondsRealtime _delay = new WaitForSecondsRealtime(_waitTime);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return _delay;
        }
    }
}
