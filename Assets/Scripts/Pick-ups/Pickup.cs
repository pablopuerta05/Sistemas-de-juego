using UnityEngine;

public class Pickup : MonoBehaviour, ICollectible
{
    public bool hasBeenCollected = false;
    private Transform player;
    private float pullSpeed;
    private bool isBeingPulled = false;

    // Método para iniciar el tirón hacia el jugador
    public void StartPull(Transform playerTransform, float speed)
    {
        player = playerTransform;
        pullSpeed = speed;
        isBeingPulled = true;
    }

    private void Update()
    {
        if (isBeingPulled && player != null)
        {
            // Mueve la pickup hacia el jugador
            transform.position = Vector2.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);

            // Auto-collect si llega al jugador
            if (Vector2.Distance(transform.position, player.position) < 0.1f)
            {
                Collect();
            }
        }
    }

    public virtual void Collect()
    {
        if (hasBeenCollected) return;

        hasBeenCollected = true;

        // Por defecto destruimos la pickup
        Destroy(gameObject);
    }
}
