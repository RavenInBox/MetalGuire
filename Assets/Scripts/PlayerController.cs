using Cinemachine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Start() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if(Input.GetKey(KeyCode.Escape)) Application.Quit();

        if (!isUp)
        {
            if (!isDetect)
            {
                InputCheck();
                InteractiveSystem();
                FlipPosition();
                DancingDu();
                SmootAnimation();
            }
            else
            {
                AnimatorController("Detect");
            }
        }

        Reset();
        ChekPoint();
    }

    #region variables
    [Header("Movement")]
    [SerializeField][Range (1, 80)] private float speedWalk;
    [SerializeField][Range (1, 80)] private float speedRun;

    [Header("Checkers")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform interactive;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask pushLayer;
    [SerializeField] private LayerMask jumpLayer;
    [SerializeField] private LayerMask warningLayer;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private LayerMask ckPointLayer;

    [Header ("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField][Range(1, 10)] private float SmootAnimationEnter;
    [SerializeField][Range(1, 10)] private float SmootAnimationExit;
    [SerializeField] private GameObject Warning, Detected;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private CamEffect cmEffect;

    [Header("Point")]
    [SerializeField] private Transform ckPoint;

    private Rigidbody2D rb;
    private float horizontal, HorizontalAsist;
    private float DanceEnter, timeToDance = 500;
    private bool rightFlip = true, inputWalk = false, isUp, isDetect = false;

    public bool isReset;

    private string
        // AxisRaw
        Hz = "Horizontal",
        // Parameters
        AnimPushF = "CrouchPush", AnimRunF = "WalkRun",
        AnimWalkF = "IdleWalk", AnimDanceF = "StartDance",
        // Animations
        AnimFrontFlip = "Front Flip", AnimIdle = "Idle", 
        AnimWalkRun = "WRTree", AnimIdleWalk = "IWTree", AnimSimpleJump = "Jump", 
        AnimPush = "Pushtree", AnimDance = "DTwek"; //AnimAtack = "Punch"
    #endregion

    #region Input
    private void InputCheck()
    {
        inputWalk = Input.GetKey(KeyCode.LeftShift);
        horizontal = Input.GetAxisRaw(Hz);
    }
    #endregion

    #region Interactive System
    private void InteractiveSystem()
    {
        if (IsGrounded())
        {
            if (!InteractiveType(jumpLayer))
            {
                if (!InteractiveType(pushLayer))
                {
                    if (horizontal == 0) Idle(); // Idle ----------------------------
                    else Move(); // Move ------------------------------------------
                } else Push(); // Push ------------------------------------------
            } else Jump(); //Jump -------------------------------------------
        }
        
        // Warnig ----------------------------------------------
        if (InteractiveType(warningLayer)) Warning.SetActive(true);
        else Warning.SetActive(false);

        // Detected --------------------------------------------
        if (InteractiveType(detectLayer)) isDetect = true;
    }
    #endregion

    #region Actions
    private void Idle()
    {
        if (timeToDance > 0)
            AnimatorController(AnimIdle);
    }

    private void Move()
    {
        if (inputWalk)
        {
            AnimatorController(AnimIdleWalk);
            ApplyMove(speedWalk);
        }
        else
        {
            AnimatorController(AnimWalkRun);
            ApplyMove(speedRun);
        }
    }

    private void Push()
    {
        AnimatorController(AnimPush);
        ApplyMove(speedWalk);
    }

    private void Jump()
    {
        switch (Type())
        {
            case 0:
                rb.velocity = new Vector2(horizontal * 40, rb.velocity.y + 15.5f);
                AnimatorController(AnimFrontFlip);
                break;
            case 1:
                rb.velocity = new Vector2(horizontal * 40, rb.velocity.y + 10f);
                AnimatorController(AnimSimpleJump);
                break;
            case 2:
                transform.position = new Vector2(transform.position.x + 1.5f, transform.position.y + 2f);
                AnimatorController("UpBox");
                break;
        }
    }

    private void ChekPoint()
    {
        if (InteractiveType(ckPointLayer)) ckPoint = Transfr();
    }
    #endregion

    #region Direction
    private void ApplyMove(float speed)
    {
        transform.position = new Vector2(transform.position.x + (speed * horizontal) * Time.deltaTime, transform.position.y);
    }

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
    public void Reset()
    {
        if (isReset)
        {
            transform.position = new Vector2(ckPoint.position.x, transform.position.y);
            Detected.SetActive(false);
            isDetect = false;
            isReset = false;
        }
    }

    private void SmootAnimation()
    {
        if (horizontal != 0)
            if (HorizontalAsist < 1)
                HorizontalAsist += SmootAnimationEnter * Time.deltaTime;

        if (horizontal == 0)
            if (HorizontalAsist > 0)
                HorizontalAsist -= SmootAnimationExit * Time.deltaTime;
    }

    private void AnimatorController(string anm)
    {
        animator.SetFloat(AnimPushF, HorizontalAsist);
        animator.SetFloat(AnimRunF, HorizontalAsist);
        animator.SetFloat(AnimWalkF, HorizontalAsist);
        animator.SetFloat(AnimDanceF, DanceEnter);

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

            AnimatorController(AnimDance);
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
    private Transform Transfr()
    {
        return Physics2D.OverlapCircle(interactive.position, 0.2f, ckPointLayer).transform;
    }
    #endregion
}
