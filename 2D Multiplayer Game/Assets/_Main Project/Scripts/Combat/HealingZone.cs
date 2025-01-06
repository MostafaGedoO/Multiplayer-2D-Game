using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image healPowerBar;

    [Header("Settings")]
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 30f;
    [SerializeField] private float TimeToRestorePowerOnIdle = 5f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int healPerTick = 3;

    private List<PlayerManager> playerManagers = new List<PlayerManager>();
    private float timer;

    private int currentPowerBar;
    private float lastHealTimer;
    
    private float cooldownTimer;
    private bool inCooldown;

    private void Awake()
    {
        currentPowerBar = maxHealPower;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (!IsServer) return;

        if (_collision.CompareTag("Player"))
        {
            if(_collision.attachedRigidbody.TryGetComponent<PlayerManager>(out PlayerManager _playerManager))
            {
                playerManagers.Add(_playerManager);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (!IsServer) return;

        if (_collision.CompareTag("Player"))
        {
            if (_collision.attachedRigidbody.TryGetComponent<PlayerManager>(out PlayerManager _playerManager))
            {
                playerManagers.Remove(_playerManager);
            }
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        if (inCooldown)
        {
            cooldownTimer += Time.deltaTime;
            currentPowerBar = (int)cooldownTimer;

            UpdatePowerParClientRpc(currentPowerBar, maxHealPower);

            if (cooldownTimer >= healCooldown)
            {
                inCooldown = false;
                currentPowerBar = maxHealPower;
                UpdatePowerParClientRpc(currentPowerBar, maxHealPower);
            }
            else
            {
                timer = healTickRate;
                return;
            }
        }

        if(playerManagers.Count == 0 )
        {
            if(currentPowerBar == maxHealPower)
            {
                return;
            }
            else
            {
                lastHealTimer += Time.deltaTime;
                if(lastHealTimer >= TimeToRestorePowerOnIdle)
                {
                    inCooldown = true;
                    cooldownTimer = currentPowerBar;
                }
            }
        }

        timer += Time.deltaTime;

        if (timer >= healTickRate)
        {
            foreach (var _manager in playerManagers)
            {
                if (_manager.Health.CurrentHealth.Value == 100) continue;

                _manager.Health.RestoreHealth(healPerTick);

                lastHealTimer = 0;
                timer = 0;
                currentPowerBar--;

                UpdatePowerParClientRpc(currentPowerBar,maxHealPower);

                if (currentPowerBar == 0)
                {
                    cooldownTimer = 0;
                    inCooldown = true;
                }
            }
        }
    }

    [ClientRpc]
    private void UpdatePowerParClientRpc(int _currentPower,int _maxPower)
    {
        healPowerBar.fillAmount = (float)_currentPower / _maxPower;
    }
}
