using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    protected EnemyStats enemy;
    protected Transform player;
    [SerializeField] private SpawnManager spawnManager;

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOfFrameAction
    {
        none,
        respawnAtEdge,
        despawn
    }

    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnAtEdge;

    protected bool spawnedOutOfFrame = false;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        spawnManager = FindAnyObjectByType<SpawnManager>();
        spawnedOutOfFrame = !spawnManager.IsWithinBoundaries(transform);
        enemy = GetComponent<EnemyStats>();
        player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // if we are currently being knocked back, then process the knockback
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            Move();
            HandleOutOfFrameAction();
        }
    }

    // if the enemy falls outside of the frame, handle it
    protected virtual void HandleOutOfFrameAction()
    {
        // handle the enemy when it is out of frame
        if (!spawnManager.IsWithinBoundaries(transform))
        {
            switch (outOfFrameAction)
            {
                case OutOfFrameAction.none:
                    break;
                case OutOfFrameAction.respawnAtEdge:
                    // if the enemy is outside the camera frame, teleport it back to the edge of the frame
                    transform.position = spawnManager.GeneratePosition();
                    break;
                case OutOfFrameAction.despawn:
                    // don't destroy if it is spawned outside the frame
                    if (!spawnedOutOfFrame)
                    {
                        Destroy(gameObject);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            spawnedOutOfFrame = false;
        }
    }

    // this is meant to be called from other scripts to create knockback
    public virtual void Knockback(Vector2 velocity, float duration)
    {
        // ignore the knockback if the duration is greater than 0
        if (knockbackDuration > 0)
        {
            return;
        }

        // begins the knockback
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

    public virtual void Move()
    {
        // constantly move the enemy towards the player
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);
    }
}
