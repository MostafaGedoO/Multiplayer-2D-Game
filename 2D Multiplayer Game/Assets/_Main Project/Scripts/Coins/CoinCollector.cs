using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if(_collision.TryGetComponent<Coin>(out Coin _coin))
        {
            int _coinValue = _coin.Collect();

            if (!IsServer) return;

            totalCoins.Value += _coinValue;
        }
    }
}
