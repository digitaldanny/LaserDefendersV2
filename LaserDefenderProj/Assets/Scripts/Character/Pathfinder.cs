using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private WaveConfigSO waveConfigSO;
    private List<Transform> waypoints;
    private int waypointIndex;
    private bool isWaveConfigSelected;

    /*
     * +-----+-----+-----+-----+-----+-----+-----+-----+
     * PUBLIC METHODS
     * +-----+-----+-----+-----+-----+-----+-----+-----+
     */

    /*
     * Set the wave configuration so that the game object knows which path to follow.
     */
    public void SetWaveConfig(WaveConfigSO setWaveConfig)
    {
        waveConfigSO = setWaveConfig;
        waypointIndex = 0;
        waypoints = waveConfigSO.GetWaypoints();
        transform.position = waypoints[waypointIndex].position;
        isWaveConfigSelected = true;
    }

    /*
     * +-----+-----+-----+-----+-----+-----+-----+-----+
     * PRIVATE METHODS
     * +-----+-----+-----+-----+-----+-----+-----+-----+
     */

    private void Awake()
    {
        isWaveConfigSelected = false;
    }

    void Update()
    {
        FollowPath();
    }

    void FollowPath()
    {
        if (!isWaveConfigSelected)
        {
            // Do not start following the path until the spawner selects which wave to follow.
            return;
        }

        if (waypointIndex < waypoints.Count)
        {
            // Move towards the next waypoint.
            // TODO - Is there a way I can ease into positions? Might make a cool effect for certain enemies..
            Vector3 targetPosition = waypoints[waypointIndex].position;
            float deltaPosition = waveConfigSO.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, deltaPosition);

            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            // Remove game object once we've reached the end of the path.
            Destroy(gameObject);
        }
    }
}
