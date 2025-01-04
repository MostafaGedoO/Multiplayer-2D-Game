using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinCollector CoinCollector { get; private set; }
    
    [Space]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private const int camPriority = 15;

    [Space]
    [SerializeField] private TextMeshProUGUI playerNameText;


    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<PlayerManager> OnAPlayerSpawn;
    public static event Action<PlayerManager> OnAPlayerDespawn;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            playerName.Value = HostSingleton.Instance.HostGameManager.NetworkServer?.GetClientUserData(OwnerClientId).UserName;
            OnAPlayerSpawn?.Invoke(this);
        }
        
       
        playerNameText.text = playerName.Value.ToString();

        if(IsOwner)
        {
           virtualCamera.Priority = camPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnAPlayerDespawn?.Invoke(this);
        }
    }
}
