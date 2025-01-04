using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private GameObject coinSprite;

    protected int coinValue;
    protected bool alreadyCollected;

    public abstract int Collect();

    private void OnEnable()
    {
        ChangeCoinValue();
    }

    protected void ChangeCoinValue()
    {
        coinValue = Random.Range(1, 16);
    }
    
    protected void SetCoinVisualState(bool _state)
    {
        coinSprite.SetActive(_state);
    }
}
