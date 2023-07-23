using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBoundaryObjectDeleter : MonoBehaviour
{
    /*
     * This script is meant to delete all game objects that leave
     * the camera boundaries.
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something hit.." + collision.gameObject.name);
        Destroy(collision.gameObject);
    }
}
