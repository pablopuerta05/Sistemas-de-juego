using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [System.Serializable]
    public class PoolDefinition
    {
        public GameObject prefab;
        public int poolSize = 10;
    }

    [Header("Chunk Configuration")]
    public List<PoolDefinition> chunkDefinitions;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;

    [Header("Optimization")]
    public float maxOpDist; //Must be greater than the lenght and width of the tilemap
    private float opDist;
    [SerializeField] private float optimizerCooldownDur;

    private Vector3 playerLastPosition;
    private float optimizerCooldown;

    private List<GameObject> activeChunks = new List<GameObject>();
    private Transform chunkParent;
    private HashSet<Vector3> spawnedPositions = new HashSet<Vector3>();

    private List<ChunkPool> chunkPools = new List<ChunkPool>();

    // Start is called before the first frame update
    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player no asignado en MapController.");
        }

        if (chunkDefinitions == null || chunkDefinitions.Count == 0)
        {
            Debug.LogError("No hay definiciones de chunks asignadas en MapController.");
        }

        playerLastPosition = player.transform.position;
        chunkParent = new GameObject("Chunks").transform;

        foreach (PoolDefinition def in chunkDefinitions)
        {
            ChunkPool pool = new ChunkPool
            {
                prefab = def.prefab,
                poolSize = def.poolSize
            };
            pool.Initialize(chunkParent);
            chunkPools.Add(pool);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    private void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string primaryDirection = GetDirectionName(moveDir);

        HashSet<string> directionsToCheck = new HashSet<string> { primaryDirection };

        if (primaryDirection.Contains("Up")) directionsToCheck.Add("Up");
        if (primaryDirection.Contains("Down")) directionsToCheck.Add("Down");
        if (primaryDirection.Contains("Left")) directionsToCheck.Add("Left");
        if (primaryDirection.Contains("Right")) directionsToCheck.Add("Right");

        foreach (string dir in directionsToCheck)
        {
            CheckAndSpawnChunk(dir);
        }
    }

    private void CheckAndSpawnChunk(string direction)
    {
        Transform directionPoint = currentChunk.transform.Find(direction);
        if (directionPoint == null)
        {
            Debug.LogWarning($"No se encontró el punto de dirección '{direction}' en el chunk '{currentChunk.name}'.");
            return;
        }

        Vector3 spawnPos = directionPoint.position;

        if (!Physics2D.OverlapCircle(spawnPos, checkerRadius, terrainMask))
        {
            ChunkSpawn(spawnPos);
        }
    }

    private string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Moving horizontally more than vertically
            if (direction.y > 0.5)
            {
                // Also moving upwards
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.y < -0.5)
            {
                // Also moving downwards
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                // Moving straight horizontally
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            // Moving vertically more than horizontally
            if (direction.x > 0.5)
            {
                // Also moving right
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if (direction.x < -0.5)
            {
                // Also moving left
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                // Moving straight vertically
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    private void ChunkSpawn(Vector3 spawnPosition)
    {
        if (spawnedPositions.Contains(spawnPosition))
        {
            return; // Ya hay un chunk aquí (aunque esté desactivado)
        }

        int rand = Random.Range(0, chunkPools.Count);
        GameObject chunk = chunkPools[rand].GetFromPool();
        chunk.transform.position = spawnPosition;
        chunk.transform.SetParent(chunkParent);

        activeChunks.Add(chunk);
        spawnedPositions.Add(spawnPosition);
        currentChunk = chunk;
    }

    private void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in activeChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
