using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private const int DEFAULT_HEALTH = 5;

    [SerializeField] private int health = DEFAULT_HEALTH;
    [SerializeField] private ParticleSystem hitEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if (damageDealer == null)
        {
            return;
        }

        // Take damage and tell damage dealer that it hit something.
        PlayHitEffect();
        TakeDamage(damageDealer);
        damageDealer.Hit();
    }

    private void TakeDamage(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void PlayHitEffect()
    {
        if (hitEffect == null)
        {
            return;
        }

        ParticleSystem explosionEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(explosionEffect, explosionEffect.main.duration + explosionEffect.main.startLifetime.constantMax);
    }
}
