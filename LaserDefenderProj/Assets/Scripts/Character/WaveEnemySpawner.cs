using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemySpawner : MonoBehaviour
{
    private const float DEFAULT_TIME_BETWEEN_WAVES = 3f;

    [SerializeField] private List<WaveConfigSO> waveConfigs;
    [SerializeField] private float timeBetweenWaves = DEFAULT_TIME_BETWEEN_WAVES;
    [SerializeField] private bool enableWaveLooping = true; // When enabled, we can loop through the list of waves multiple times. 

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        do
        {
            foreach (WaveConfigSO waveConfig in waveConfigs)
            {
                StartCoroutine(SpawnEnemies(waveConfig));
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        } while (enableWaveLooping);
    }

    IEnumerator SpawnEnemies(WaveConfigSO waveConfig)
    {
        for (int enemyIndex = 0; enemyIndex < waveConfig.GetEnemyCount(); enemyIndex++)
        {
            GameObject enemyGameObject = Instantiate(
                waveConfig.GetEnemyPrefab(enemyIndex),      /* Game Object to instantiate */
                waveConfig.GetStartingWaypoint().position,  /* Starting position */
                Quaternion.Euler(0, 0, 180),                /* Rotation = 180 degrees so enemies point down */
                transform                                   /* Transform of the parent object to instantiate this game object into (enemySpawner) */
            );
            enemyGameObject.GetComponent<Pathfinder>().SetWaveConfig(waveConfig);

            yield return new WaitForSeconds(waveConfig.GetRandomSpawnTime());
        }
    }
}
