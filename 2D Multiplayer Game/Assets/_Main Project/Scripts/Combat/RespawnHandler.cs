using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private PlayerManager playerPrefab;
    [SerializeField] private float coinsPercentToLoseOnDie;

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
        float _remainingCoins = _player.CoinCollector.totalCoins.Value - (_player.CoinCollector.totalCoins.Value * (coinsPercentToLoseOnDie / 100)); 
        Destroy(_player.gameObject);
        StartCoroutine(SpawnThePlayer(_player.OwnerClientId,_remainingCoins));
    }

    private IEnumerator SpawnThePlayer(ulong _playerClientId,float _remainingCoins)
    {
        yield return null;

        PlayerManager _PlayerManager = Instantiate(playerPrefab,SpwanPoint.GetRandomSpwanPoint(),Quaternion.identity);
        _PlayerManager.NetworkObject.SpawnAsPlayerObject(_playerClientId);
        _PlayerManager.CoinCollector.totalCoins.Value = (int)_remainingCoins;
    }
}
