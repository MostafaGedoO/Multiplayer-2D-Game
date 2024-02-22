using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab;
    [Space]
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [Space]
    [SerializeField] private LayerMask LayerMask;

    private CircleCollider2D[] colliderBuffer = new CircleCollider2D[1];
    private float coinRadius = 0.5f;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            for(int i = 0; i < maxCoins; i++)
            {
                SpawnCoin();
            }
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin _coin = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
        _coin.GetComponent<NetworkObject>().Spawn();
        _coin.OnCollected += Coin_OnCollected;
    }

    private void Coin_OnCollected(RespawningCoin _coin)
    {
        _coin.transform.position = GetSpawnPoint();
        _coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        while(true)
        {
            float _x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            float _y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 _spawnPoint = new Vector2(_x, _y);
            int _collideNum = Physics2D.OverlapCircleNonAlloc(_spawnPoint, coinRadius, colliderBuffer, LayerMask);
            if(_collideNum == 0)
            {
                return _spawnPoint;
            }
        }
    }
}
