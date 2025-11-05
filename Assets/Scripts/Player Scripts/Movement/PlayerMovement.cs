using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 currentMoveInput;

    //References
    Rigidbody2D rb;
    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = currentMoveInput * player.CurrentMoveSpeed;
    }

    public void Move(Vector2 direction)
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }

        currentMoveInput = direction;
    }
}
