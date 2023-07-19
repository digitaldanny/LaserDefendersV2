using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /* 
     * +-----+-----+-----+-----+-----+
     * CONSTANTS
     * +-----+-----+-----+-----+-----+
     */
    private const float DEFAULT_MOVEMENT_SPEED_FLOAT = 12f;
    private readonly Vector2 VIEWPORT_BOUNDARIES_MIN = new Vector2(0, 0);
    private readonly Vector2 VIEWPORT_BOUNDARIES_MAX = new Vector2(1, 1);

    /*
     * +-----+-----+-----+-----+-----+
     * PRIVATE
     * +-----+-----+-----+-----+-----+
     */

    [Header("Movement")]
    [SerializeField] private float moveSpeed = DEFAULT_MOVEMENT_SPEED_FLOAT; // Player movement speed multiplier
    private Vector2 rawUserInputMove;   // Raw user input for "movement" (WASD)
    private Vector2 minScreenBounds;    // Minimum boundaries of the World Point
    private Vector2 maxScreenBounds;    // Maximum boundaries of the World Point
    [SerializeField] private int bottomBoundaryPadding;  // Padding to stop player from traveling through the UI

    // Script references
    private Shooter shooter;            // Object to instantiate projectiles

    /*
     * +-----+-----+-----+-----+-----+
     * PRIVATE METHODS
     * +-----+-----+-----+-----+-----+
     */
    private void Awake()
    {
        shooter = GetComponent<Shooter>(); 
    }

    private void Start()
    {
        InitMovementBoundaries();
    }

    void Update()
    {
        Move();
    }
    
    void OnMove(InputValue value)
    {
        rawUserInputMove = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        if (shooter == null)
        {
            return;
        }

        shooter.SetEnableShooting(value.isPressed);
    }

    /*
     * Initialize player movement boundaries based on camera size.
     */
    private void InitMovementBoundaries()
    {
        Camera camera = Camera.main;

        // Assign screen boundaries for player movement.
        minScreenBounds = camera.ViewportToWorldPoint(new Vector2(VIEWPORT_BOUNDARIES_MIN.x, VIEWPORT_BOUNDARIES_MIN.y));
        maxScreenBounds = camera.ViewportToWorldPoint(new Vector2(VIEWPORT_BOUNDARIES_MAX.x, VIEWPORT_BOUNDARIES_MAX.y));
    }

    private void Move()
    {
        Vector2 deltaPosition = rawUserInputMove * moveSpeed * Time.deltaTime;
        Vector2 newPosition = new Vector2();

        // Clamp the player movement to min/max boundaries determined by the
        // camera dimensions and player size.
        // 
        // More on player size:
        // At scale (1,1,1), half of the player could go off screen. 
        // Solution is to pad the screen boundaries by half of the player's scale.
        Vector2 playerSize = new Vector2(transform.localScale.x / 2f, transform.localScale.y / 2f);
        newPosition.x = Mathf.Clamp(transform.position.x + deltaPosition.x, minScreenBounds.x + playerSize.x, maxScreenBounds.x - playerSize.x);
        newPosition.y = Mathf.Clamp(transform.position.y + deltaPosition.y, minScreenBounds.y + playerSize.y + bottomBoundaryPadding, maxScreenBounds.y - playerSize.y);

        transform.position = newPosition;
    }
}
