using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    public float pullSpeed = 5f;
    private CircleCollider2D playerCollector;

    private void Start()
    {
        playerCollector = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        // Ajustar radio si el jugador tiene CurrentMagnet
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            playerCollector.radius = player.CurrentMagnet;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si es una pickup, iniciar el tirón
        if (collision.gameObject.TryGetComponent(out Pickup pickup))
        {
            pickup.StartPull(transform, pullSpeed);
        }
    }
}
