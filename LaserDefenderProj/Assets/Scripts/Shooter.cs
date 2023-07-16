using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileLifetime = 3f;

    private bool enableShooting;
    private Coroutine firingCoroutine;

    /*
     * +-----+-----+-----+-----+-----+
     * PUBLIC METHODS
     * +-----+-----+-----+-----+-----+
     */
    public void SetEnableShooting(bool enable)
    {
        enableShooting = enable;
    }

    /*
     * +-----+-----+-----+-----+-----+
     * PRIVATE METHODS
     * +-----+-----+-----+-----+-----+
     */
    void Start()
    {
        enableShooting = false;
        firingCoroutine = null;
    }

    void Update()
    {
        Fire();
    }

    void Fire()
    {
        if ((enableShooting) && (firingCoroutine == null))
        {
            Debug.Log("Starting coroutine");
            // Start coroutine to fire projectiles
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if ((!enableShooting) && (firingCoroutine != null))
        {
            // Not currently firing, and we have already started a firing coroutine.
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject projectile = Instantiate(
                projectilePrefab,      /* Game Object to instantiate */
                transform.position,    /* Starting position will be the parent */
                Quaternion.identity    /* Specify no rotation */
            );
            
            // Projectile shoots up
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = transform.up * projectileSpeed;
            }

            // After some time, destroy the projectile game object
            Destroy(projectile, projectileLifetime);
            yield return new WaitForSeconds(fireRate);
        }
    }
}
