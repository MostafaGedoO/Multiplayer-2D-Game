using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    private Dictionary<ulong,string> clientIDToAuthID = new Dictionary<ulong,string>();
    private Dictionary<string,UserData> authIDToUserData = new Dictionary<string,UserData>();

    public event Action<string> OnClientLeft;
    public NetworkServer()
    {
        networkManager = NetworkManager.Singleton;

        //Setting a listner for the connection of players
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest _request, NetworkManager.ConnectionApprovalResponse _response)
    {
        //Getting the string for the byte[] in the request
        string _requestText = System.Text.Encoding.UTF8.GetString(_request.Payload);

        //Getting out the UserData that we passed in the connection of the host and client
        UserData _userData = JsonUtility.FromJson<UserData>(_requestText);

        //Saving Player Data
        clientIDToAuthID[_request.ClientNetworkId] = _userData.AuthId;
        authIDToUserData[_userData.AuthId] = _userData;

        //Approving the connection
        _response.Position = SpwanPoint.GetRandomSpwanPoint();
        _response.Approved = true;
        _response.CreatePlayerObject = true;
    }

    private void NetworkManager_OnServerStarted()
    {
        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong _clientId)
    {
        //Removing Player Data on Disconnection
        if(clientIDToAuthID.TryGetValue(_clientId,out string _authId))
        {
            clientIDToAuthID.Remove(_clientId);
            authIDToUserData.Remove(_authId);
            OnClientLeft?.Invoke(_authId);
        }
    }

    public UserData GetClientUserData(ulong _clientId)
    {
        if (clientIDToAuthID.TryGetValue(_clientId, out string _authId))
        {
            return authIDToUserData[_authId];
        }
        return null;
    }

    public void Dispose()
    {
        if(networkManager !=  null)
        {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnServerStarted -= NetworkManager_OnServerStarted;
            networkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
}
