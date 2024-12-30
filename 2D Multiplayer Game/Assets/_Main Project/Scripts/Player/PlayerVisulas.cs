using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisulas : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private const int camPriority = 15;

    [Space]
    [SerializeField] private TextMeshProUGUI playerNameText;

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            playerName.Value = HostSingleton.Instance.HostGameManager.NetworkServer?.GetClientUserData(OwnerClientId).UserName;
        }
       
        playerNameText.text = playerName.Value.ToString();

        if(IsOwner)
        {
           virtualCamera.Priority = camPriority;
        }
    }
}
