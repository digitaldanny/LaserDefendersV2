using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRateBase = 0.25f;
    [SerializeField] private float fireRateVariance = 0f;
    [SerializeField] private float fireRateMin = 0.2f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileLifetime = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip shootingSound;
    [SerializeField] private AudioSource_e audioSource;
    private AudioPlayer audioPlayer;

    private Coroutine firingCoroutine;

    /*
     * +-----+-----+-----+-----+-----+
     * PUBLIC METHODS
     * +-----+-----+-----+-----+-----+
     */

    /*
     * This function starts a coroutines that will continuously fire a single projectile 
     * after the delays configured for the class.
     * 
     * NOTE: This function is mostly useful for the player character who will just hold 
     *  down the Fire button and expect shooting to happen at the max speed.
     *  
     *  param  enableShooting       Start shooting when true, and stop shooting when false.
     */
    public void FireContinuously(bool enableShooting)
    {
        if ((enableShooting) && (firingCoroutine == null))
        {
            // Start coroutine to fire projectiles
            firingCoroutine = StartCoroutine(FireContinuouslyRoutine());
        }
        else if ((!enableShooting) && (firingCoroutine != null))
        {
            // Not currently firing, and we have already started a firing coroutine.
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    /*
     * This function fires a single bullet immediately WITHOUT considering the time
     * elapsed since the last shot.
     *  
     *  param  enableShooting       Start shooting when true, and stop shooting when false.
     */
    public void FireSingleShot()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,      /* Game Object to instantiate */
            transform.position,    /* Starting position will be the parent */
            Quaternion.identity    /* Specify no rotation */
        );

        // Projectile shoots and plays the audio
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.up * projectileSpeed;
        }

        audioPlayer.PlaySound(shootingSound, audioSource);

        // After some time, destroy the projectile game object
        Destroy(projectile, projectileLifetime);
    }

    /*
     * +-----+-----+-----+-----+-----+
     * PRIVATE METHODS
     * +-----+-----+-----+-----+-----+
     */

    private void Awake()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }

    void Start()
    {
        firingCoroutine = null;
    }

    /*
     * This coroutine will fire a single bullet after the delays configured for the class.
     * 
     * NOTE: This function is mostly useful for the player character who will just hold 
     *  down the Fire button and expect shooting to happen at the max speed.
     */
    IEnumerator FireContinuouslyRoutine()
    {
        while (true)
        {
            FireSingleShot();

            // Wait some time before shooting another projectile
            float timeToNextProjectile = Random.Range(fireRateBase - fireRateVariance, fireRateBase + fireRateVariance);
            timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, fireRateMin, float.MaxValue);
            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }
}
