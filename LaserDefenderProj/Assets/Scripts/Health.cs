using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private const int DEFAULT_HEALTH = 5;

    [SerializeField] private int health = DEFAULT_HEALTH;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if (damageDealer == null)
        {
            return;
        }

        // Take damage and tell damage dealer that it hit something.
        health -= damageDealer.GetDamage();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        damageDealer.Hit();
    }
}
