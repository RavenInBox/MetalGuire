using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        InputCheck();
        InteractiveSystem();
        FlipPosition();
        DancingDu();
        SmootAnimation();
    }

    #region variables
    [SerializeField] private float speedWalk, speedRun;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck, interactive;
    [SerializeField] private LayerMask groundLayer, pushLayer, jumpLayer;

    [Space]
    [SerializeField] private float SmootEnter, SmootExit;
    [SerializeField] private Animator animator;
    [SerializeField] private string idle, walk, run, jump, atck, pushmove, Dance;

    private double typeJump;
    private float horizontal, HorizontalAsist, DanceEnter, timeToDance = 500;
    private bool rightFlip = true, inputWalk = false;

    private string
        Hz = "Horizontal", AnimPush = "CrouchPush", AnimRun = "WalkRun",
        AnimWalk = "IdleWalk", AnimDance = "StartDance", AnimFrontFlip = "Front Flip";
    #endregion

    #region Input
    private void InputCheck()
    {
        inputWalk = Input.GetKey(KeyCode.LeftShift);
        horizontal = Input.GetAxisRaw(Hz);
    }

    private void InteractiveSystem()
    {
        if (IsGrounded())
        {
            if (!InteractiveType(jumpLayer))
            {
                if (!InteractiveType(pushLayer))
                {
                    // Free move-----------------------------------------
                    if (horizontal != 0)
                    {
                        if (!inputWalk)
                        {
                            AnimatorController(walk);
                            ApplyMove(speedRun);
                        }
                        else
                        {
                            AnimatorController(run);
                            ApplyMove(speedWalk);
                        }
                    }
                    else
                    {
                        if (timeToDance > 0)
                            AnimatorController(idle);
                    }
                }
                else
                {
                    // interactive Push-------------------------------------
                    AnimatorController(pushmove);
                    ApplyMove(speedWalk);
                }
            }
            else
            {
                //Jump Action----------------------------------------------
                switch (Type())
                {
                    case 0:
                        AnimatorController(AnimFrontFlip);
                        break;
                }
            }
        }
        else
        {
            // Is fall //
        }
    }
    #endregion

    #region Direction
    private void ApplyMove(float speed) => rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

    private void FlipPosition()
    {
        if (rightFlip && horizontal < 0f || !rightFlip && horizontal > 0f)
        {
            rightFlip = !rightFlip;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    #endregion

    #region Animation
    private void SmootAnimation()
    {
        if (horizontal != 0)
            if (HorizontalAsist < 1)
                HorizontalAsist += SmootEnter * Time.deltaTime;

        if (horizontal == 0)
            if (HorizontalAsist > 0)
                HorizontalAsist -= SmootExit * Time.deltaTime;
    }

    private void AnimatorController(string anm)
    {
        animator.SetFloat(AnimPush, HorizontalAsist);
        animator.SetFloat(AnimRun, HorizontalAsist);
        animator.SetFloat(AnimWalk, HorizontalAsist);
        animator.SetFloat(AnimDance, DanceEnter);

        animator.Play(anm);
    }

    private void DancingDu()
    {
        if (Input.anyKeyDown || InteractiveType(pushLayer))
        {
            timeToDance = 500f;
            DanceEnter = 0;
        }

        if (timeToDance > 0)
            timeToDance -= 5f * Time.deltaTime;

        if (timeToDance <= 0)
        {
            if (DanceEnter <= 1)
                DanceEnter += 2.5f * Time.deltaTime;

            AnimatorController("DTwek");
        }
    }
    #endregion

    #region Checkers
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 1f, groundLayer);
    }

    private bool InteractiveType(LayerMask layer)
    {
        return Physics2D.OverlapCircle(interactive.position, 0.2f, layer);
    }

    private int Type()
    {
        return Physics2D.OverlapCircle(interactive.position, 0.2f, jumpLayer)
            .gameObject.GetComponent<TypeJump>().typejump;
    }
    #endregion
}
