using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody == null) return;

        if(collision.attachedRigidbody.TryGetComponent<Health>(out Health _health))
        {
            _health.TakeDamage(damage);
        }
    }
}
