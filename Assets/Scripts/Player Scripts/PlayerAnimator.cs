using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator Animator;
    private PlayerMovement PlayerMovement;
    private SpriteRenderer PlayerRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovement.moveDir.x != 0 || PlayerMovement.moveDir.y != 0)
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
        if (PlayerMovement.lastHorizontalVector < 0)
        {
            PlayerRenderer.flipX = true;
        }
        else
        {
            PlayerRenderer.flipX = false;
        }
    }
}
