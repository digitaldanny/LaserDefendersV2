using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private const int DEFAULT_HEALTH = 5;

    [Header("Health")]
    [SerializeField] private int health = DEFAULT_HEALTH;

    [Header("Visual")]
    [SerializeField] private ParticleSystem hitEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip audioOnDamageTaken;
    [SerializeField] private AudioClip audioOnDeath;
    [SerializeField] private AudioSource_e audioSource;
    private AudioPlayer audioPlayer;

    [Header("Camera Shake")]
    [SerializeField] private bool applyCameraShakeOnDamage = false;
    private CameraShake cameraShake;

    private void Awake()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
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
        PlayExplosionAnimation();
        ShakeCamera();
        TakeDamage(damageDealer);
        damageDealer.Hit();
    }

    private void TakeDamage(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        if (health <= 0)
        {
            // If character has died, destroy the game object and play death sound.
            audioPlayer.PlaySound(audioOnDeath, audioSource);
            Destroy(gameObject);
        }
        else
        {
            // Play damage taken sound.
            audioPlayer.PlaySound(audioOnDamageTaken, audioSource);
        }
    }

    private void PlayExplosionAnimation()
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
