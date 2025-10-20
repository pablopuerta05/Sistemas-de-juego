using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator Animator;
    private InputHandler inputHandler;
    private SpriteRenderer PlayerRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        inputHandler = GetComponent<InputHandler>();
        PlayerRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputHandler.moveDir.x != 0 || inputHandler.moveDir.y != 0)
        {
            Animator.SetBool("isWalking", true);

            spriteDirection();
        }
        else
        {
            Animator.SetBool("isWalking", false);
        }
    }

    void spriteDirection()
    {
        if (inputHandler.lastHorizontalVector < 0)
        {
            PlayerRenderer.flipX = true;
        }
        else
        {
            PlayerRenderer.flipX = false;
        }
    }
}
