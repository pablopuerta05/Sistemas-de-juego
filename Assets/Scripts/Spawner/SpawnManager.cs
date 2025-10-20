using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    int currentWaveIndex; // the index of the current wave (it starts at 0)
    int currentWaveSpawnCount = 0; // track how many enemies current wave has spawned

    public WaveData[] data;
    public Camera referenceCamera;

    [Tooltip("if there are more than this number of enemies, stop spawning any more. For performance")]
    public int maximumEnemyCount = 300;

    private float spawnTimer; // timer used to determine when to spawn the next group of enemy
    private float currentWaveDuration;

    public Camera ReferenceCamera => referenceCamera;
    public int MaximumEnemyCount => maximumEnemyCount;

    private IItemFactory enemiesFactory;

    private void Awake()
    {
        enemiesFactory = new EnemiesFactory();
    }

    private void Update()
    {
        // updates the spawn timer at every frame
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;

        if (spawnTimer <= 0)
        {
            // check if we are ready to move on to the next wave
            if (HasWaveEnded())
            {
                currentWaveIndex++;
                currentWaveDuration = currentWaveSpawnCount = 0;

                // if we have gone through all the waves, disable this component
                if (currentWaveIndex >= data.Length)
                {
                    Debug.Log("all waves have been spawned. shutting down", this);
                    enabled = false;
                }

                return;
            }

            // do not spawn enemies if we do not meet the conditions to do so
            if (!CanSpawn())
            {
                spawnTimer += data[currentWaveIndex].GetSpawnInterval();
                return;
            }

            // get the array of enemies that we are spawning for this tick
            GameObject[] spawns = data[currentWaveIndex].GetSpawns(EnemyStats.count);

            // loop through and spawn all the prefabs
            foreach (GameObject prefab in spawns)
            {
                // stop spawning enemies if we exceed the limit
                if (!CanSpawn())
                {
                    continue;
                }

                // spawn the enemy
                enemiesFactory.Create(prefab, GeneratePosition(), Quaternion.identity);
                currentWaveSpawnCount++;
            }

            // regenerates the spawn timer
            spawnTimer += data[currentWaveIndex].GetSpawnInterval();
        }
    }

    // do we meet the conditions to be able to continue spawning?
    public bool CanSpawn()
    {
        // do not spawn anymore if we exceed the max limit
        if (HasExceededMaxEnemies())
        {
            return false;
        }

        WaveData currentWave = data[currentWaveIndex];

        // do not spawn if we exceeded the max spawns for the wave
        if (currentWaveSpawnCount > currentWave.totalSpawns)
        {
            return false;
        }

        // do not spawn if we exceeded the wave's duration
        if (currentWaveDuration > currentWave.duration)
        {
            return false;
        }

        return true;
    }

    // allows other scripts to check if we have exceeded the maximum number of enemies
    public bool HasExceededMaxEnemies()
    {
        return EnemyStats.count > maximumEnemyCount;
    }

    public bool HasWaveEnded()
    {
        WaveData currentWave = data[currentWaveIndex];

        // if waveDuration is one of the exit conditions, check how long the wave has been running
        // if current wave duration is not greater than wave duration, do not exit yet
        if ((currentWave.exitConditions & WaveData.ExitCondition.waveDuration) > 0)
        {
            if (currentWaveDuration < currentWave.duration)
            {
                return false;
            }
        }

        // if reachedTotalSpawns is one of the exit conditions, check if we have spawned enough enemies. If not, return false
        if ((currentWave.exitConditions & WaveData.ExitCondition.reachedTotalSpawns) > 0)
        {
            if (currentWaveSpawnCount < currentWave.totalSpawns)
            {
                return false;
            }
        }

        // otherwise, if kill all is checked, we have to make sure there are no more enemies first
        if (currentWave.mustKillAll && EnemyStats.count > 0)
        {
            return false;
        }

        return true;
    }

    private void Reset()
    {
        referenceCamera = Camera.main;
    }

    // creates a new location where we can place the enemy at
    public Vector3 GeneratePosition()
    {
        // if there is no reference camera, then get one
        if (!referenceCamera)
        {
            referenceCamera = Camera.main;
        }

        // give a warning if the camera is not ortographic
        if (!referenceCamera.orthographic)
        {
            Debug.LogWarning("The reference camera is not ortographic, this will cause enemy spawns to appear within camera boundaries");
        }

        // generate a position outside of camera boundaries using 2 random numbers
        float x = Random.Range(0f, 1f);
        float y = Random.Range(0f, 1f);

        // then, randomly choose whether we want to round the X or the Y value
        switch (Random.Range(0, 2))
        {
            case 0:
                return referenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(x), y));
            case 1:
                return referenceCamera.ViewportToWorldPoint(new Vector3(x, Mathf.Round(y)));
            default:
                return referenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(x), y));
        }
    }

    // checking if the enemy is within the camera's boundaries
    public bool IsWithinBoundaries(Transform checkedObject)
    {
        // get the camera to check if we are within boundaries
        Camera camera = referenceCamera ? referenceCamera : Camera.main;

        Vector2 viewport = camera.WorldToViewportPoint(checkedObject.position);
        
        if (viewport.x < 0 || viewport.x > 1)
        {
            return false;
        }
        if (viewport.y < 0 || viewport.y > 1)
        {
            return false;
        }

        return true;
    }
}
