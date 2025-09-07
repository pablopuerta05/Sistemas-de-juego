using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private Vector3 playerLastPosition;

    //[Header("Optimization")]
    public List<GameObject> spawnedChunks;
    private GameObject LatestChunk;
    //public float maxOpDist; //Must be greater than the lenght and width of the tilemap
    //private float opDist;
    //private float optimizerCooldown;
    //[SerializeField] private float optimizerCooldownDur;

    // Start is called before the first frame update
    void Start()
    {
        playerLastPosition = player.transform.position; ;
    }

    // Update is called once per frame
    void Update()
    {
        chunkChecker();
        //chunkOptimizer();
    }

    void chunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(directionName);

        // Check additional adjacent directions for diagonal chunks
        if (directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if (directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            chunkSpawn(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionName(Vector3 direction)
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

    void chunkSpawn(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        LatestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(LatestChunk);
    }

    //void chunkOptimizer()
    //{
    //    optimizerCooldown -= Time.deltaTime;

    //    if (optimizerCooldown <= 0)
    //    {
    //        optimizerCooldown = optimizerCooldownDur;
    //    }
    //    else
    //    {
    //        return;
    //    }

    //    foreach (GameObject chunk in spawnedChunks)
    //    {
    //        opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
    //        if (opDist > maxOpDist)
    //        {
    //            chunk.SetActive(false);
    //        }
    //        else
    //        {
    //            chunk.SetActive(true);
    //        }
    //    }
    //}
}
