using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyPlayersNumberText;
    [SerializeField] private Button joinButton;

    private LobbiesList lobbiesList;
    private Lobby lobby;

    public void InitializeLobby(LobbiesList _lobbiesList,Lobby _lobby)
    {
        lobbiesList = _lobbiesList;
        lobby = _lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayersNumberText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

        joinButton.onClick.AddListener(JoinLobby);
    }

    private void JoinLobby()
    {
        lobbiesList.JoinLobbyAsync(lobby);
    }
}
