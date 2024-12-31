using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        PlayerManager[] _players = FindObjectsByType<PlayerManager>(sortMode: FindObjectsSortMode.None);
        foreach (PlayerManager _player in _players)
        {
           PlayerManager_OnAPlayerSpawn(_player);
        }

        PlayerManager.OnAPlayerSpawn += PlayerManager_OnAPlayerSpawn;
        PlayerManager.OnAPlayerDespawn += PlayerManager_OnAPlayerDespawn;

        Debug.Log("Events Handling Done");
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        PlayerManager.OnAPlayerSpawn -= PlayerManager_OnAPlayerSpawn;
        PlayerManager.OnAPlayerDespawn -= PlayerManager_OnAPlayerDespawn;
    }

    private void PlayerManager_OnAPlayerSpawn(PlayerManager _player)
    {
        _player.Health.OnDie += (_health) => HandlePlayerDeath(_player);
    }

    private void PlayerManager_OnAPlayerDespawn(PlayerManager _player)
    {
        _player.Health.OnDie -= (_health) => HandlePlayerDeath(_player);
    }

    private void HandlePlayerDeath(PlayerManager _player)
    {
        Debug.Log("Player: " + _player.OwnerClientId + " is dead");
        Destroy(_player.gameObject);
        StartCoroutine(SpawnThePlayer(_player.OwnerClientId));
    }

    private IEnumerator SpawnThePlayer(ulong _playerClientId)
    {
        yield return null;

        NetworkObject _networkObject = Instantiate(playerPrefab,SpwanPoint.GetRandomSpwanPoint(),Quaternion.identity);
        _networkObject.SpawnAsPlayerObject(_playerClientId);
    }
}
