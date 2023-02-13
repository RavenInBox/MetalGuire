using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        animationSystem = GetComponent<AnimationSystem>();
    }

    private void Update()
    {
        InputCheck();
    }

    #region variables
    [Header("Movement")]
    [SerializeField][Range(1, 80)] private float speedWalk;
    [SerializeField][Range(1, 80)] private float speedRun;
    [SerializeField][Range(-50, 50)] private float gravityForce;
    [SerializeField][Range(-50, 50)] private float XgravityForce;
    [SerializeField][Range(0, 5000)] private float jumpPower;

    private float horizontal;
    private bool rightFlip = true, SlowWalk = false;

    readonly private string Horzontal = "Horizontal";

    // Animation
    private AnimationSystem animationSystem;
    readonly private string
        //animation
        AnimFrontFlip = "Front Flip", AnimIdle = "Idle",
        AnimWalkRun = "WRTree", AnimIdleWalk = "IWTree", AnimSimpleJump = "Jump",
        AnimPush = "Pushtree", AnimTryPush = "tryPush", AnimDetect = "Detect"; //AnimAtack = "Punch"
    #endregion

    #region Input
    private void InputCheck()
    {
        SlowWalk = Input.GetKey(KeyCode.LeftShift);
        horizontal = Input.GetAxisRaw(Horzontal);

        //float targetSpeed = horizontal !=0 ? speedRun : speedWalk;
    }
    #endregion

    #region Actions
    private void Move()
    {
        if (horizontal != 0)
        {
            if (SlowWalk)
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

    private IEnumerator Jump(int j)
    {
        switch (j)
        {
            case 0:
                gravityForce = 30;
                XgravityForce = 30;
                transform.position += new Vector3(0, jumpPower * Time.deltaTime, 0);
                animationSystem.AnimatorController(AnimFrontFlip);
                yield return new WaitForSeconds(.6f);
                XgravityForce = 0;
                gravityForce = -24;
                break;
            case 1:
                animationSystem.AnimatorController(AnimSimpleJump);
                break;
        }
    }

    private void TryPush()
    {
        animationSystem.AnimatorController(AnimTryPush);
        ApplyMove(speedWalk);
    }

    private void Warning()
    {
        animationSystem.AnimatorController(AnimDetect);
    }

    private void Gravity()
    {
        Physics.gravity = new Vector2(XgravityForce, gravityForce);
    }
    #endregion

    #region Calls
    public void CallMove() => Move();
    public void CallPush() => Push();
    public void CallTryPush() => TryPush();
    public void CallJump(int j) => StartCoroutine(Jump(j));
    public void CallWarning() => Warning();
    public void CallGravity() => Gravity();
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