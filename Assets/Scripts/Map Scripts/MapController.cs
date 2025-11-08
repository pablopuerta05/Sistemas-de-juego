using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MapController : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> chunkPrefabs;
    [SerializeField] private float chunkSize = 10f;
    [SerializeField] private int renderRadius = 1;

    [Header("Optimization")]
    [SerializeField] private float maxOpDist = 40f;
    [SerializeField] private float optimizerCooldown = 2f;

    private Vector2Int playerLastChunk;
    private float cooldown;
    private Transform chunkParent;

    // Pool nativo de Unity
    private List<ObjectPool<GameObject>> chunkPools = new List<ObjectPool<GameObject>>();
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        if (!player)
        {
            Debug.LogError("Player no asignado en MapController");
            enabled = false;
            return;
        }

        chunkParent = new GameObject("Chunks").transform;

        // Crear un pool para cada prefab
        foreach (var prefab in chunkPrefabs)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(prefab, chunkParent);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: Destroy,
                defaultCapacity: 20
            );

            chunkPools.Add(pool);
        }

        UpdateChunks(); // generar los primeros
    }

    void Update()
    {
        Vector2Int playerChunk = GetChunkCoord(player.transform.position);

        if (playerChunk != playerLastChunk)
        {
            playerLastChunk = playerChunk;
            UpdateChunks();
        }

        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            cooldown = optimizerCooldown;
            OptimizeChunks();
        }
    }

    private Vector2Int GetChunkCoord(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    private Vector3 CoordToWorld(Vector2Int coord)
    {
        return new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
    }

    private void UpdateChunks()
    {
        Vector2Int playerChunk = GetChunkCoord(player.transform.position);
        HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

        for (int x = -renderRadius; x <= renderRadius; x++)
        {
            for (int y = -renderRadius; y <= renderRadius; y++)
            {
                Vector2Int coord = playerChunk + new Vector2Int(x, y);
                neededChunks.Add(coord);

                if (!activeChunks.ContainsKey(coord))
                {
                    SpawnChunk(coord);
                }
            }
        }

        // Liberar chunks que ya no se necesitan
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!neededChunks.Contains(kvp.Key))
            {
                ReleaseChunk(kvp.Key);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var c in toRemove)
            activeChunks.Remove(c);
    }

    private void SpawnChunk(Vector2Int coord)
    {
        int rand = Random.Range(0, chunkPools.Count);
        GameObject chunk = chunkPools[rand].Get();

        chunk.transform.position = CoordToWorld(coord);
        chunk.name = $"Chunk_{coord.x}_{coord.y}";

        activeChunks[coord] = chunk;
    }

    private void ReleaseChunk(Vector2Int coord)
    {
        if (activeChunks.TryGetValue(coord, out GameObject chunk))
        {
            // Encuentra el pool correspondiente al prefab
            foreach (var pool in chunkPools)
            {
                if (pool.CountActive > 0 && pool.CountInactive >= 0)
                {
                    pool.Release(chunk);
                    break;
                }
            }
        }
    }

    private void OptimizeChunks()
    {
        foreach (var kvp in activeChunks)
        {
            float dist = Vector3.Distance(player.transform.position, kvp.Value.transform.position);
            kvp.Value.SetActive(dist < maxOpDist);
        }
    }
}
