using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const float DEFAULT_TIME_BETWEEN_WAVES = 3f;

    [SerializeField] private List<WaveConfigSO> waveConfigs;
    [SerializeField] private float timeBetweenWaves = DEFAULT_TIME_BETWEEN_WAVES;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        foreach (WaveConfigSO waveConfig in waveConfigs)
        {
            StartCoroutine(SpawnEnemies(waveConfig));
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnEnemies(WaveConfigSO waveConfig)
    {
        for (int enemyIndex = 0; enemyIndex < waveConfig.GetEnemyCount(); enemyIndex++)
        {
            GameObject enemyGameObject = Instantiate(
                waveConfig.GetEnemyPrefab(enemyIndex),      /* Game Object to instantiate */
                waveConfig.GetStartingWaypoint().position,  /* Starting position */
                Quaternion.identity,                        /* Specify no rotation */
                transform                                   /* Transform of the parent object to instantiate this game object into (enemySpawner) */
            );
            enemyGameObject.GetComponent<Pathfinder>().SetWaveConfig(waveConfig);

            yield return new WaitForSeconds(waveConfig.GetRandomSpawnTime());
        }
    }
}
