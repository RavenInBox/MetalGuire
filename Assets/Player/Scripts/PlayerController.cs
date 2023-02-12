using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        detectSystem = GetComponent<DetectSystem>();
        animationSystem = GetComponent<AnimationSystem>();
    }

    void Update()
    {
        InputCheck();
    }

    #region variables
    [Header("Movement")]
    [SerializeField][Range(1, 80)] private float speedWalk;
    [SerializeField][Range(1, 80)] private float speedRun;

    private Rigidbody rb;
    private float horizontal;
    private bool rightFlip = true, slowWalk = false;

    readonly private string Horzontal = "Horizontal";

    private DetectSystem detectSystem;

    // Animation
    private AnimationSystem animationSystem;
    readonly private string
        //machine state
        move = "move", push = "push", jump = "jump", warning = "warning", tryPush = "trypush",
        //animation
        AnimFrontFlip = "Front Flip", AnimIdle = "Idle",
        AnimWalkRun = "WRTree", AnimIdleWalk = "IWTree", AnimSimpleJump = "Jump",
        AnimPush = "Pushtree", AnimTryPush = "tryPush", AnimDetect = "Detect"; //AnimAtack = "Punch"
    #endregion

    #region Input
    private void InputCheck()
    {
        slowWalk = Input.GetKey(KeyCode.LeftShift);
        horizontal = Input.GetAxisRaw(Horzontal);
    }
    #endregion

    #region Actions
    private void Move()
    {
        if (horizontal != 0)
        {
            if (slowWalk)
            {
                animationSystem.AnimatorController(AnimIdleWalk);
                ApplyMove(speedWalk);
            }
            else
            {
                animationSystem.AnimatorController(AnimWalkRun);
                ApplyMove(speedRun);
            }
        }
        else
        {
            animationSystem.AnimatorController(AnimIdle);
        }
    }

    private void Push()
    {
        animationSystem.AnimatorController(AnimPush);
        ApplyMove(speedWalk);
    }

    private void Jump()
    {
        switch (detectSystem.JumpT)
        {
            case 0:
                rb.velocity = new Vector2(horizontal * 40, rb.velocity.y + 15.5f);
                animationSystem.AnimatorController(AnimFrontFlip);
                break;
            case 1:
                rb.velocity = new Vector2(horizontal * 40, rb.velocity.y + 10f);
                animationSystem.AnimatorController(AnimSimpleJump);
                break;
        }
    }

    private void Warning()
    {
        animationSystem.AnimatorController(AnimDetect);
    }

    private void TryPush()
    {
        animationSystem.AnimatorController(AnimTryPush);
        ApplyMove(speedWalk);
    }
    #endregion

    #region Calls
    public void CallSMachine(string State)
    {
        if (State == move) Move();
        if (State == push) Push();
        if (State == jump) Jump();
        if (State == tryPush) TryPush();
        if (State == warning) Warning();
    }
    #endregion

    #region Movement
    private void ApplyMove(float speed)
    {
        transform.position = new Vector2(transform.position.x + speed * horizontal * Time.deltaTime, transform.position.y);
        Flip();
    }

    private void Flip()
    {
        if (rightFlip && horizontal < 0f || !rightFlip && horizontal > 0f)
        {
            rightFlip = !rightFlip;
            transform.rotation = Quaternion.Inverse(transform.rotation);
        }
    }
    #endregion
}