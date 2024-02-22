using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    private Vector3 lastPosition;

    private void Update()
    {
        if(transform.position != lastPosition)
        {
            SetCoinVisualState(true);
        }

        lastPosition = transform.position;
    }

    public override int Collect()
    {
        if(!IsServer)
        {
            SetCoinVisualState(false);
            return 0;
        }

        if(alreadyCollected) 
        {
            return 0; 
        }

        alreadyCollected = true;
        OnCollected?.Invoke(this);
        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
