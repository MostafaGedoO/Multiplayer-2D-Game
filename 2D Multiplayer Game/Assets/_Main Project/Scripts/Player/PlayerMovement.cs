using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform tankBody;
    [SerializeField] private Rigidbody2D rb;
    [Space]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningRate = 30f;

    private Vector2 moveVictor;

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            inputReader.OnMoveEvent += HandleMovement;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            inputReader.OnMoveEvent -= HandleMovement;
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            tankBody.Rotate(0,0, moveVictor.x * -turningRate * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            rb.velocity = tankBody.up * moveSpeed * moveVictor.y;
        }
    }

    private void HandleMovement(Vector2 _moveVictor)
    {
        moveVictor = _moveVictor;
    }
}
