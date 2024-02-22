using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            health.CurrentHealth.OnValueChanged += OnHealthChange;
            OnHealthChange(0, health.CurrentHealth.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient)
            health.CurrentHealth.OnValueChanged -= OnHealthChange;
    }

    private void OnHealthChange(int _oldHealth,int _newHealth)
    {
        healthBarImage.fillAmount = _newHealth / (float)health.MaxHealth;
    }
}
