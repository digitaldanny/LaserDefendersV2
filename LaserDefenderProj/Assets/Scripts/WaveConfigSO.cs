using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave Config", fileName = "New Wave Config")]
public class WaveConfigSO : ScriptableObject
{
    private const float DEFAULT_MOVE_SPEED = 12f;
    private const float DEFAULT_SPAWN_TIME_DELAY = 1f;
    private const float DEFAULT_SPAWN_TIME_VARIANCE = 0f;

    [SerializeField] private Transform pathPrefab; // TODO - Should take a specific type rather than a generic Transform object
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] private float moveSpeed = DEFAULT_MOVE_SPEED;
    [SerializeField] private float spawnTimeDelay = DEFAULT_SPAWN_TIME_DELAY;
    [SerializeField] private float spawnTimeVariance = DEFAULT_SPAWN_TIME_VARIANCE;

    public Transform GetStartingWaypoint()
    {
        return pathPrefab.GetChild(0);
    }

    public List<Transform> GetWaypoints()
    {
        List<Transform> waypoints = new List<Transform>();

        foreach (Transform waypoint in pathPrefab)
        {
            waypoints.Add(waypoint);    
        }

        return waypoints;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetEnemyCount()
    {
        return enemyPrefabs.Count;
    }

    public GameObject GetEnemyPrefab(int enemyIndex)
    {
        return enemyPrefabs[enemyIndex];
    }    

    public float GetRandomSpawnTime()
    {
        float spawnTime = Random.Range(spawnTimeDelay - spawnTimeVariance, spawnTimeDelay + spawnTimeVariance);
        return Mathf.Clamp(spawnTime, 0f, float.MaxValue);
    }
}
