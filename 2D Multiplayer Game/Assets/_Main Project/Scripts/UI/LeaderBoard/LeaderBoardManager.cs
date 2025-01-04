using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardManager : NetworkBehaviour
{
   [SerializeField] private Transform leaderboardContainer;
   [SerializeField] private LeaderBoardItem leaderBoardItemPrefab;

   private NetworkList<LeaderBoardState> leaderBoardStates;
   private List<LeaderBoardItem> leaderBoardItems = new List<LeaderBoardItem>();
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
         if (!leaderBoardItems.Any(x => x.PlayerStateID == _changeevent.Value.ClientID))
         {
            LeaderBoardItem _leaderBoardItem = Instantiate(leaderBoardItemPrefab, leaderboardContainer);
            _leaderBoardItem.Initialize(_changeevent.Value.ClientID, _changeevent.Value.PlayerName.ToString(),_changeevent.Value.Coins);
            leaderBoardItems.Add(_leaderBoardItem);
         }
      }
      else if (_changeevent.Type == NetworkListEvent<LeaderBoardState>.EventType.Remove)
      {
         LeaderBoardItem _leaderBoardItem = leaderBoardItems.FirstOrDefault(x => x.PlayerStateID == _changeevent.Value.ClientID);
         if (_leaderBoardItem != null)
         {
            _leaderBoardItem.transform.parent = null;
            leaderBoardItems.Remove(_leaderBoardItem);
            Destroy(_leaderBoardItem.gameObject);
         }
      }
      else if (_changeevent.Type == NetworkListEvent<LeaderBoardState>.EventType.Value)
      {
         LeaderBoardItem _leaderBoardItem = leaderBoardItems.FirstOrDefault(x => x.PlayerStateID == _changeevent.Value.ClientID);
         if (_leaderBoardItem != null)
         {
            _leaderBoardItem.UpdateCoins(_changeevent.Value.Coins);
         }
      }

      leaderBoardItems.Sort((x,y) => y.PlayerStateCoins.CompareTo(x.PlayerStateCoins));
      
      for (int i = 0; i < leaderBoardItems.Count ; i++)
      {
         leaderBoardItems[i].transform.SetSiblingIndex(i);
         leaderBoardItems[i].UpdateText();
         leaderBoardItems[i].gameObject.SetActive(i <= 7); //to show only 8 players on the leaderboard
      }
      
      LeaderBoardItem _localPlayerItem = leaderBoardItems.LastOrDefault(x => x.PlayerStateID == NetworkManager.Singleton.LocalClientId);
      if (_localPlayerItem != null)
      {
         if (_localPlayerItem.transform.GetSiblingIndex() >= 8)
         {
            leaderboardContainer.GetChild(7).gameObject.SetActive(false);
            _localPlayerItem.gameObject.SetActive(true);
         }
      }
   }

   private void PlayerManager_OnAPlayerSpawn(PlayerManager _player)
   {
      leaderBoardStates.Add(new LeaderBoardState(){ClientID = _player.OwnerClientId , PlayerName = _player.playerName.Value , Coins = 0});
      _player.CoinCollector.totalCoins.OnValueChanged += (_oldCoins,_newCoins) => HandleCoinChange(_player.OwnerClientId, _newCoins);
   }

   private void PlayerManager_OnAPlayerDespawn(PlayerManager _player)
   {
      if(leaderBoardStates == null) return;
      
      foreach (var playerState in leaderBoardStates)
      {
         if(playerState.ClientID != _player.OwnerClientId) continue;
         leaderBoardStates.Remove(playerState);
      }
      
      _player.CoinCollector.totalCoins.OnValueChanged -= (_oldCoins,_newCoins) => HandleCoinChange(_player.OwnerClientId, _newCoins);
   }
   
   private void HandleCoinChange(ulong _playerClientId,int _coins)
   {
      for (int i = 0; i < leaderBoardStates.Count; i++)
      {
         if(leaderBoardStates[i].ClientID != _playerClientId) continue;
         
         leaderBoardStates[i] = new LeaderBoardState(){ClientID = leaderBoardStates[i].ClientID, PlayerName = leaderBoardStates[i].PlayerName, Coins = _coins };
         
         return;
      }
   }
}
