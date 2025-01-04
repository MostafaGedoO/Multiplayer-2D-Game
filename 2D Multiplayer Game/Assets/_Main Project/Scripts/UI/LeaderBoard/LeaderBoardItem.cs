using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardItem : MonoBehaviour
{
    [SerializeField] private Color myColor;
    [SerializeField] private TextMeshProUGUI leaderBoardItemText;
    public ulong PlayerStateID { get; private set; }
    public int PlayerStateCoins { get; private set; }
    
    private string playerStateName;

    public void Initialize(ulong _playerStateID, string _playerStateName, int _playerStateCoins)
    {
        PlayerStateID = _playerStateID;
        playerStateName = _playerStateName;

        if (_playerStateID == NetworkManager.Singleton.LocalClientId)
        {
            leaderBoardItemText.color = myColor;
        }
        
        UpdateCoins(_playerStateCoins);
    }

    public void UpdateCoins(int _coins)
    {
        PlayerStateCoins = _coins;
        UpdateText();
    }
    
    public void UpdateText()
    {
        leaderBoardItemText.text = $"{ transform.GetSiblingIndex() + 1 }. {playerStateName} ({PlayerStateCoins})";
    }
}
