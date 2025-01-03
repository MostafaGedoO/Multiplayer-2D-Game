using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardManager : NetworkBehaviour
{
   [SerializeField] private Transform leaderboardContainer;
   [SerializeField] private LeaderBoardItem leaderBoardItemPrefab;

   private NetworkList<LeaderBoardState> leaderBoardStates;

   private void Awake()
   {
      leaderBoardStates = new NetworkList<LeaderBoardState>();
   }

   public override void OnNetworkSpawn()
   {
      if (IsClient)
      {
         leaderBoardStates.OnListChanged += OnLeaderBoardStateChanged;

         foreach (var _playerState in leaderBoardStates)
         {
            OnLeaderBoardStateChanged(new NetworkListEvent<LeaderBoardState>(){Type = NetworkListEvent<LeaderBoardState>.EventType.Add,Value = _playerState });
         }
      }
      
      if(!IsServer) return;
      
      PlayerManager[] _players = FindObjectsByType<PlayerManager>(sortMode: FindObjectsSortMode.None);
      foreach (PlayerManager _player in _players)
      {
         PlayerManager_OnAPlayerSpawn(_player);
      }

      PlayerManager.OnAPlayerSpawn += PlayerManager_OnAPlayerSpawn;
      PlayerManager.OnAPlayerDespawn += PlayerManager_OnAPlayerDespawn;
   }

   public override void OnNetworkDespawn()
   {
      if (IsClient)
      {
         leaderBoardStates.OnListChanged -= OnLeaderBoardStateChanged;
      }
      
      if(!IsServer) return;
      
      PlayerManager.OnAPlayerSpawn -= PlayerManager_OnAPlayerSpawn;
      PlayerManager.OnAPlayerDespawn -= PlayerManager_OnAPlayerDespawn;
   }
   
   private void OnLeaderBoardStateChanged(NetworkListEvent<LeaderBoardState> _changeevent)
   {
      if (_changeevent.Type == NetworkListEvent<LeaderBoardState>.EventType.Add)
      {
         Instantiate(leaderBoardItemPrefab, leaderboardContainer);
      }
   }

   private void PlayerManager_OnAPlayerSpawn(PlayerManager _player)
   {
      leaderBoardStates.Add(new LeaderBoardState(){ClientID = _player.OwnerClientId , PlayerName = _player.playerName.Value , Coins = 0});
   }
   
   private void PlayerManager_OnAPlayerDespawn(PlayerManager _player)
   {
      if(leaderBoardStates == null) return;
      
      foreach (var playerState in leaderBoardStates)
      {
         if(playerState.ClientID != _player.OwnerClientId) continue;
         leaderBoardStates.Remove(playerState);
      }
   }
}
