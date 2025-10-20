using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if the other game object has the ICollectible interface
        if (collision.gameObject.TryGetComponent(out ICollectible collectible))
        {
            // gets the rigid body 2d component of the item
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            // vector2 pointing from the item to the player
            Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(forceDirection * pullSpeed);

            // if it does, call the collect method
            collectible.Collect();
        }
    }
}
