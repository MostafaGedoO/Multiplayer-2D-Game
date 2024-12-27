using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemsContanier;
    [SerializeField] private LobbyItem lobbyItemPrefab;

    private bool isJoining;
    private bool isRefershing;

    private void OnEnable()
    {
        RefershLobbiesList();   
    }

    public async void RefershLobbiesList()
    {
        if(isRefershing) return;
        isRefershing = true;

        try
        {
            QueryLobbiesOptions _queryLobbiesOptions = new QueryLobbiesOptions();
            _queryLobbiesOptions.Count = 10;
            _queryLobbiesOptions.Filters = new List<QueryFilter>()
            {
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots,op: QueryFilter.OpOptions.GT,value:"0"), //Has Room
                new QueryFilter(field: QueryFilter.FieldOptions.IsLocked,op:QueryFilter.OpOptions.EQ,value:"0") //Lobby Not Locked
            };

            QueryResponse _queryResponse = await Lobbies.Instance.QueryLobbiesAsync(_queryLobbiesOptions);

            foreach(Transform _child in lobbyItemsContanier)
            {
                Destroy(_child.gameObject);
            }

            foreach(Lobby _lobby in _queryResponse.Results)
            {
                LobbyItem _lobbyItem = Instantiate(lobbyItemPrefab,lobbyItemsContanier);
                _lobbyItem.InitializeLobby(this, _lobby);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.LogException(e);
        }

        isRefershing = false;
    }

    public async void JoinLobbyAsync(Lobby _lobby)
    {
        if(isJoining) return;
        
        isJoining = true;

        try
        {
            Lobby _joinLobby = await Lobbies.Instance.JoinLobbyByIdAsync(_lobby.Id);
            string _joinCode = _joinLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.ClientGameManager.StartClientAsync(_joinCode);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogException(e);
            return;
        }

        isJoining = false;
    }

    
}
