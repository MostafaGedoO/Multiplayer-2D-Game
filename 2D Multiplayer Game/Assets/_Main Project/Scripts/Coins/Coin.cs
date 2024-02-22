using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private GameObject coinSprite;

    protected int coinValue = 1;
    protected bool alreadyCollected;

    public abstract int Collect();

    public void SetValue(int _coinValue)
    {
        coinValue = _coinValue;
    }

    protected void SetCoinVisualState(bool _state)
    {
        coinSprite.SetActive(_state);
    }
}
