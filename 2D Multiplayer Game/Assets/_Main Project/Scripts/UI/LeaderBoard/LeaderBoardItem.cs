using System;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardItem : MonoBehaviour
{
   [SerializeField] private Transform leaderboardContainer;
   [SerializeField] private LeaderBoardItem leaderBoardItemPrefab;

   private NetworkList<LeaderBoardState> leaderBoardStates;

   private void Awake()
   {
      leaderBoardStates = new NetworkList<LeaderBoardState>();
   }
}
