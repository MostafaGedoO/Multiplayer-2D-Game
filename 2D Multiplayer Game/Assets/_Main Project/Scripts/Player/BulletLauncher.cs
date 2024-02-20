using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletLauncher : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [Space]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject clientBullet;
    [SerializeField] private GameObject severBullet;

    private bool fire;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            inputReader.OnFireEvent += InputReader_OnFireEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
            inputReader.OnFireEvent -= InputReader_OnFireEvent;
    }

    private void InputReader_OnFireEvent(bool _fireState)
    {
        fire = _fireState;
    }

    void Update()
    {
        if(IsOwner & fire)
        {
            SpwanClientBullet(bulletSpawnPoint.position, bulletSpawnPoint.transform.up);
            SpawnServerBulletServerRpc(bulletSpawnPoint.position, bulletSpawnPoint.transform.up);
        }
    }

    [ServerRpc]
    private void SpawnServerBulletServerRpc(Vector3 _spawnPoint, Vector3 _direction)
    {
        Instantiate(severBullet, _spawnPoint, Quaternion.identity).transform.up = _direction;
        SpawnClientBulletClientRpc(_spawnPoint, _direction);
    }

    [ClientRpc]
    private void SpawnClientBulletClientRpc(Vector3 _spawnPoint, Vector3 _direction)
    {
        if (!IsOwner) 
        {
            Instantiate(clientBullet, _spawnPoint, Quaternion.identity).transform.up = _direction;
        }
    }

    private void SpwanClientBullet(Vector3 _spawnPoint, Vector3 _direction)
    {
        Instantiate(clientBullet, _spawnPoint, Quaternion.identity).transform.up = _direction;
    }
}
