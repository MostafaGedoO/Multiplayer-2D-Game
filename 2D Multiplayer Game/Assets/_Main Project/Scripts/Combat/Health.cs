using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public int MaxHealth { get; private set; } = 100;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool isDead;
    public event Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            CurrentHealth.Value = MaxHealth;
        }
    }

    public void TakeDamage(int _damageValue)
    {
        UpdateHealth(-_damageValue);
    }

    public void RestoreHealth(int _healValue)
    {
        UpdateHealth(_healValue);
    }

    public void UpdateHealth(int _healthValue)
    {
        if(!isDead)
        {
            int _newHealth = CurrentHealth.Value + _healthValue;
            CurrentHealth.Value = Math.Clamp(_newHealth,0,MaxHealth);

            if(CurrentHealth.Value == 0) 
            {
                isDead = true;
                OnDie?.Invoke(this);
            }
        }
    }

}
