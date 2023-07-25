using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SideswiperEnemySpawner : MonoBehaviour
{
    #region Types
    enum SpawnSide_e
    {
        SPAWN_STATE_TOP,
        SPAWN_STATE_BOTTOM,
        SPAWN_STATE_LEFT,
        SPAWN_STATE_RIGHT
    }
    #endregion Types

    #region Data
    [SerializeField] private GameObject sideswiperPrefab;
    [SerializeField] private GameObject warningPrefab;

    [Header("Spawn Delays")]
    [SerializeField] private float timeToDisplayWarning;
    [SerializeField] private float timeBetweenSpawnBase;
    [SerializeField] private float timeBetweenSpawnVariance;
    [SerializeField] private float timeBetweenSpawnMin;
    Coroutine spawnCoroutine;

    [Header("Spawn Positions")]
    [SerializeField][Range(0, 1)] private float xPlaneViewportBoundaryPadding = 0.2f;  // Sideswiper must stay within (screenLeft + padding, screenRight - padding)
    [SerializeField][Range(0, 1)] private float yPlaneViewportBoundaryPadding = 0.2f;  // Sideswiper must stay within (screenBottom + padding, screenTop - padding)
    
    private Vector2 worldViewMin;
    private Vector2 worldViewMax;
    private SpawnSide_e spawnSide;

    [SerializeField][Tooltip("Select any sides you do not want enemy to spawn from")] 
    private List<SpawnSide_e> spawnSideIgnoreList;

    [Header("Other")]
    [SerializeField] private bool enableSpawn; // Sideswiper can only spawn with this set to true
    private Camera camera;

    [Header("DEBUG - SPAWN VISUALIZER")]
    [SerializeField] private bool enableSpawnVisualizer = true;
    [SerializeField] private GameObject debugPrefab;
    [SerializeField] private Color colorSpawnPathRange = Color.yellow;
    [SerializeField] private Color colorSpawnPathChosen = Color.green;
    [SerializeField] private Color colorSpawnBoundaries = Color.red;
    #endregion Data

    /*
     * +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
     * MONOBEHAVIOR OVERRIDES
     * +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
     */
    #region MonobehaviorOverrides
    private void Awake()
    {
        camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnCoroutine = null;
        SetWorldViewBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Debug_EnableDebugPrefab(enableSpawnVisualizer);
        Debug_DrawPlaneBoundaryLines();

        // TODO - Enable multiple coroutines to start so that multiple sideswipers can spawn at a time..
        if (enableSpawn && (spawnCoroutine == null))
        {
            spawnCoroutine = StartCoroutine(SpawnSideSwiperRoutine());
        }
    }
    #endregion MonobehaviorOverrides

    /*
     * +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
     * PRIVATE METHODS
     * +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
     */
    #region PrivateMethods
    /*
     * Coroutine to spawn in the sideswiper prefab after a delay. Following steps will happen..
     * - Generate a start point to instantiate the game object.
     * - Generate a rotation so that the game object will reach the other end of the screen.
     * - Instantiate the warning symbol and play alert sound.
     * - After a delay, instantiate the sideswiper.
     */
    IEnumerator SpawnSideSwiperRoutine()
    {
        while (true)
        {
            SpawnSide_e spawnSide = ChooseRandomSpawnSide(spawnSideIgnoreList);
            Vector2 spawnPoint = GenerateSpawnPointWithinBoundaries(spawnSide);
            Vector2 direction = GenerateRotationWithinBoundaries(spawnPoint, spawnSide);

            // Spawn a sideswiper or debug prefab with the selected spawn point + rotation
            if (enableSpawnVisualizer)
            {
                // Debug prefab
                debugPrefab.transform.position = spawnPoint;
                debugPrefab.transform.up = direction;
            }
            else
            {
                // Warning 
                GameObject iconWarning = Instantiate(
                    warningPrefab,   /* Game Object to instantiate */
                    spawnPoint,         /* Starting position */
                    Quaternion.identity,   /* Make the player's local "up" direction equal to the direction */
                    transform           /* Transform of the parent object to instantiate this game object into (enemySpawner) */
                );
                iconWarning.transform.up = direction;

                yield return new WaitForSeconds(timeToDisplayWarning);
                Destroy(iconWarning);

                // Sideswiper
                GameObject enemyGameObject = Instantiate(
                    sideswiperPrefab,   /* Game Object to instantiate */
                    spawnPoint,         /* Starting position */
                    Quaternion.identity,   /* Make the player's local "up" direction equal to the direction */
                    transform           /* Transform of the parent object to instantiate this game object into (enemySpawner) */
                );
                enemyGameObject.transform.up = direction;
            }

            yield return new WaitForSeconds(timeBetweenSpawnBase);
        }
    }

    /*
     * Chooses a random side of the screen to spawn the sideswiper from.
     * 
     * param    spawnSideIgnoreList     List of SpawnSide_e enums that should be ignored.
     * 
     *  NOTE: Choosing all 4 spawn side options would cause an infinite loop.
     *        Correct solution to disable spawning would be to set the enableSpawn variable to false.
     */
    private SpawnSide_e ChooseRandomSpawnSide(List<SpawnSide_e> spawnSideIgnoreList)
    {
        SpawnSide_e spawnSide;

        do
        {
            // Generate a random spawn choice, making sure we ignore the side options we don't want.
            spawnSide = Util.RandomEnumValue<SpawnSide_e>();
        } while (spawnSideIgnoreList.Contains(spawnSide));

        return spawnSide;
    }

    /*
     * Generate a random start point that the sideswiper will be spawned into, clamping the spawnpoint
     * to boundaries.
     * 
     * @param    spawnSide   Indicates whether the prefab will be positioned on the top, bottom, left, or right side of the screen.
     * 
     * @return  Position vector that the enemy could spawn in from.   
     */
    private Vector2 GenerateSpawnPointWithinBoundaries(SpawnSide_e spawnSide)
    {
        Vector2 spawnPoint;

        switch (spawnSide)
        {
            case SpawnSide_e.SPAWN_STATE_TOP:
                spawnPoint = new Vector2(
                    Random.Range(GetWorldViewSpawnStartMin().x, GetWorldViewSpawnStartMax().x), /* x */
                    GetWorldViewMax().y); /* y */
                break;

            case SpawnSide_e.SPAWN_STATE_BOTTOM:
                spawnPoint = new Vector2(
                    Random.Range(GetWorldViewSpawnStartMin().x, GetWorldViewSpawnStartMax().x), /* x */
                    GetWorldViewMin().y); /* y */
                break;

            case SpawnSide_e.SPAWN_STATE_LEFT:
                spawnPoint = new Vector2(
                    GetWorldViewMin().x, /* x */
                    Random.Range(GetWorldViewSpawnStartMin().y, GetWorldViewSpawnStartMax().y)); /* y */
                break;

            case SpawnSide_e.SPAWN_STATE_RIGHT:
                spawnPoint = new Vector2(
                    GetWorldViewMax().x, /* x */
                    Random.Range(GetWorldViewSpawnStartMin().y, GetWorldViewSpawnStartMax().y)); /* y */
                break;

            default:
                spawnPoint = new Vector2(0, 0);
                Debug.Log("ERROR: Entered invalid spawnSide case.");
                break;
        }

        return spawnPoint;
    }

    /*
     * Generate a random endpoint that the sideswiper will move towards and calculate the Quaternion
     * to orient the prefab so that it reaches that endpoint if traveling in a straight line.
     * 
     * @param    spawnPoint  Starting position that the prefab will be instantiated.
     * @param    spawnSide   Indicates whether the prefab will be positioned on the top, bottom, left, or right side of the screen.
     * 
     * @return  Direction vector to orient the character's local up vector towards.
     */
    private Vector2 GenerateRotationWithinBoundaries(Vector2 spawnPoint, SpawnSide_e spawnSide)
    {
        Vector2 endPoint;
        Vector2 endPointMin;
        Vector2 endPointMax;

        // Pick a random end point
        switch (spawnSide)
        {
            case SpawnSide_e.SPAWN_STATE_BOTTOM:
                endPointMin = new Vector2(GetWorldViewSpawnStartMin().x, GetWorldViewMax().y);
                endPointMax = new Vector2(GetWorldViewSpawnStartMax().x, GetWorldViewMax().y);
                break;

            case SpawnSide_e.SPAWN_STATE_TOP:
                endPointMin = new Vector2(GetWorldViewSpawnStartMin().x, GetWorldViewMin().y);
                endPointMax = new Vector2(GetWorldViewSpawnStartMax().x, GetWorldViewMin().y);
                break;

            case SpawnSide_e.SPAWN_STATE_RIGHT:
                endPointMin = new Vector2(GetWorldViewMin().x, GetWorldViewSpawnStartMin().y);
                endPointMax = new Vector2(GetWorldViewMin().x, GetWorldViewSpawnStartMax().y);
                break;

            case SpawnSide_e.SPAWN_STATE_LEFT:
                endPointMin = new Vector2(GetWorldViewMax().x, GetWorldViewSpawnStartMin().y);
                endPointMax = new Vector2(GetWorldViewMax().x, GetWorldViewSpawnStartMax().y);
                break;

            default:
                endPointMin = new Vector2(0, 0);
                endPointMax = new Vector2(0, 0);
                Debug.Log("ERROR: Entered invalid spawnSide case.");
                break;
        }

        endPoint = new Vector2(Random.Range(endPointMin.x, endPointMax.x), Random.Range(endPointMin.y, endPointMax.y));

        // Debug - Draw the min, max, and chosen path lines
        if (enableSpawnVisualizer)
        {
            Debug.DrawLine(spawnPoint, endPoint, Color.green, timeBetweenSpawnBase);
            Debug.DrawLine(spawnPoint, endPointMin, Color.yellow, timeBetweenSpawnBase);
            Debug.DrawLine(spawnPoint, endPointMax, Color.yellow, timeBetweenSpawnBase);
        }

        // Calculate direction from the spawn / end vectors
        return new Vector2(endPoint.x - spawnPoint.x, endPoint.y - spawnPoint.y);
    }

    /*
     * This function draws minimum / maximum boundaries that the enemy could spawn within.
     */
    private void Debug_DrawPlaneBoundaryLines()
    {
        if (!enableSpawnVisualizer)
        {
            return;
        }

        Vector2 minSpawnBoundaries = GetWorldViewSpawnStartMin();
        Vector2 maxSpawnBoundaries = GetWorldViewSpawnStartMax();
        Vector2 minScreenBoundaries = GetWorldViewMin();
        Vector2 maxScreenBoundaries = GetWorldViewMax();

        // Min X boundary starting from (xmin, 0) -> (xmin, 1)
        Debug.DrawLine(
            new Vector2(minSpawnBoundaries.x, minScreenBoundaries.y),
            new Vector2(minSpawnBoundaries.x, maxScreenBoundaries.y),
            colorSpawnBoundaries);

        // Max X boundary starting from (xmax, 0) -> (xmax, 1)
        Debug.DrawLine(
            new Vector2(maxSpawnBoundaries.x, minScreenBoundaries.y),
            new Vector2(maxSpawnBoundaries.x, maxScreenBoundaries.y),
            colorSpawnBoundaries);

        // Min Y boundary starting from (0, ymin) -> (1, ymin)
        Debug.DrawLine(
            new Vector2(minScreenBoundaries.x, minSpawnBoundaries.y),
            new Vector2(maxScreenBoundaries.x, minSpawnBoundaries.y),
            colorSpawnBoundaries);

        // Max Y boundary starting from (0, ymax) -> (1, ymax)
        Debug.DrawLine(
            new Vector2(minScreenBoundaries.x, maxSpawnBoundaries.y),
            new Vector2(maxScreenBoundaries.x, maxSpawnBoundaries.y),
            colorSpawnBoundaries);
    }

    /*
     * Enables or disables the debug game object to visualize where sideswiper would spawn in.
     */
    private void Debug_EnableDebugPrefab(bool enable)
    {
        debugPrefab.SetActive(enable);
    }

    /*
     * Minimum X, Y World View coordinates of the camera.
     */
    private void SetWorldViewBoundaries()
    {
        worldViewMin = camera.ViewportToWorldPoint(new Vector2(0, 0)); // get bottom left point
        worldViewMax = camera.ViewportToWorldPoint(new Vector2(1, 1)); // get top right point
    }

    /*
     * Minimum X, Y World View coordinates that the sideswiper can spawn in from.
     */
    private Vector2 GetWorldViewSpawnStartMin()
    {
        return camera.ViewportToWorldPoint(new Vector2(
                    0f + xPlaneViewportBoundaryPadding,
                    0f + yPlaneViewportBoundaryPadding));
    }

    /*
     * Maximum X, Y World View coordinates that the sideswiper can spawn in from.
     */
    private Vector2 GetWorldViewSpawnStartMax()
    {
        return camera.ViewportToWorldPoint(new Vector2(
                    1f - xPlaneViewportBoundaryPadding,
                    1f - yPlaneViewportBoundaryPadding));
    }

    /*
     * Returns bottom left World View point of the screen.
     */
    private Vector2 GetWorldViewMin()
    {
        return worldViewMin;
    }

    /*
     * Returns top right World View point of the screen.
     */
    private Vector2 GetWorldViewMax()
    {
        return worldViewMax;
    }
    #endregion PrivateMethods
}
