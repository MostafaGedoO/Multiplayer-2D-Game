using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation allocation;

    private int maxPlayers = 20;
    public string JoinCode {  get; private set; }

    private string lobbyId;

    public NetworkServer NetworkServer;

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

        string _playerNeme = PlayerPrefs.GetString(PlayerNameHandler.playerNameKey, "Player" + UnityEngine.Random.Range(100, 1000));

        try
        {
            CreateLobbyOptions _lobbyOptions = new CreateLobbyOptions();
            _lobbyOptions.IsPrivate = false;
            _lobbyOptions.Data = new Dictionary<string, DataObject> {{"JoinCode",new DataObject(visibility: DataObject.VisibilityOptions.Member,value: JoinCode)}};

            Lobby _lobby = await Lobbies.Instance.CreateLobbyAsync($"{_playerNeme}'s Room", maxPlayers, _lobbyOptions);
            lobbyId = _lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        //Making a networkServer for it to listen for the connection Approval
        NetworkServer = new NetworkServer();

        //Setting the Player Data
        UserData _userDate = new UserData { UserName = _playerNeme, AuthId = AuthenticationService.Instance.PlayerId };

        //Making the byte[] the we can pass to the networkManager
        string _dataJson = JsonUtility.ToJson(_userDate);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(_dataJson);

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

    public async void Dispose()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeartBeatLobby));
        
        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }

            lobbyId = string.Empty;
        }
        
        NetworkServer?.Dispose();
    }
}
