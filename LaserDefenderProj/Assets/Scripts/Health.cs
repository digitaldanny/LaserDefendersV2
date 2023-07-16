using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private const int DEFAULT_HEALTH = 5;

    [SerializeField] private int health = DEFAULT_HEALTH;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private bool applyCameraShakeOnDamage = false;

    private CameraShake cameraShake;

    private void Awake()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if (damageDealer == null)
        {
            return;
        }

        // Take damage and tell damage dealer that it hit something.
        PlayHitEffect();
        ShakeCamera();
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

    private void PlayHitEffect()
    {
        if (hitEffect == null)
        {
            return;
        }

        ParticleSystem explosionEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(explosionEffect, explosionEffect.main.duration + explosionEffect.main.startLifetime.constantMax);
    }

    private void ShakeCamera()
    {
        if ((cameraShake == null) || !applyCameraShakeOnDamage)
        {
            return;
        }

        cameraShake.Play();
    }
}
