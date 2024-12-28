using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    private NetworkManager networkManager;

    public NetworkServer()
    {
        networkManager = NetworkManager.Singleton;

        //Setting a listner for the connection of players
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest _request, NetworkManager.ConnectionApprovalResponse _response)
    {
        //Getting the string for the byte[] in the request
        string _requestText = System.Text.Encoding.UTF8.GetString(_request.Payload);

        //Getting out the UserData that we passed in the connection of the host and client
        UserData _userData = JsonUtility.FromJson<UserData>(_requestText);

        Debug.Log(_userData.userName);

        //Approving the connection
        _response.Approved = true;
        _response.CreatePlayerObject = true;
    }
}
