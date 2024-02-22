using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletLauncher : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [Space]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject clientBullet;
    [SerializeField] private GameObject severBullet;
    [Space]
    [SerializeField] private Collider2D playerColider;
    [SerializeField] private GameObject muzzleFlash;
    [Space]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;

    private bool fire;
    private float lastBulletTime;
    private float muzzleFlashTimer;

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
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if (muzzleFlashTimer <= 0)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (IsOwner & fire)
        {
            if (Time.time < (1 / fireRate) + lastBulletTime) return;

            SpwanClientBullet(bulletSpawnPoint.position, bulletSpawnPoint.transform.up);
            SpawnServerBulletServerRpc(bulletSpawnPoint.position, bulletSpawnPoint.transform.up);

            lastBulletTime = Time.time;
        }
    }

    [ServerRpc]
    private void SpawnServerBulletServerRpc(Vector3 _spawnPoint, Vector3 _direction)
    {
        GameObject _bullet = Instantiate(severBullet, _spawnPoint, Quaternion.identity);
        _bullet.transform.up = _direction;
        Physics2D.IgnoreCollision(_bullet.GetComponent<Collider2D>(), playerColider);

        if (_bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D _rb))
        {
            _rb.velocity = _rb.transform.up * bulletSpeed;
        }

        SpawnClientBulletClientRpc(_spawnPoint, _direction);
    }

    [ClientRpc]
    private void SpawnClientBulletClientRpc(Vector3 _spawnPoint, Vector3 _direction)
    {
        if (!IsOwner) 
        {
            SpwanClientBullet(_spawnPoint, _direction);
        }
    }

    private void SpwanClientBullet(Vector3 _spawnPoint, Vector3 _direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject _bullet = Instantiate(clientBullet, _spawnPoint, Quaternion.identity);
        _bullet.transform.up = _direction;
        Physics2D.IgnoreCollision(_bullet.GetComponent<Collider2D>(),playerColider);

        if(_bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D _rb))
        {
            _rb.velocity = _rb.transform.up * bulletSpeed;
        }
    }
}
