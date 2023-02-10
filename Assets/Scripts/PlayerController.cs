using Cinemachine;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetPoint();
        InputCheck();
    }

    private void FixedUpdate()
    {
        if (!isDetect)
        {
            InteractiveSystem();
            WarningAndDetectSatuts();
            SmootAnimationParameters();
        }
        else Camera();
    }

    #region variables
    [Header("Movement")]
    [SerializeField][Range (1, 80)] private float speedWalk;
    [SerializeField][Range (1, 80)] private float speedRun;
    private Rigidbody2D rb;
    private float horizontal, HorizontalAsist;

    [Header("Checkers")]
    [SerializeField] private GroundChek groundchek;
    [SerializeField] private InteractiveChek interactivechek;
    private bool rightFlip = true, slowWalk = false, moving = false, isDetect = false;

    [Header ("Animation")]
    [SerializeField][Range(1, 10)] private float SmootAnimationFactor;
    [SerializeField] private GameObject Warning, Detected;
    private Animator animator;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float camDistance;

    [Header("Point")]
    [SerializeField] private Transform ckPoint;

    private string
        // AxisRaw
        Horzontal = "Horizontal",
        // Parameters
        AnimPushF = "CrouchPush", AnimRunF = "WalkRun",
        AnimWalkF = "IdleWalk",
        // Animations
        AnimFrontFlip = "Front Flip", AnimIdle = "Idle", 
        AnimWalkRun = "WRTree", AnimIdleWalk = "IWTree", AnimSimpleJump = "Jump", 
        AnimPush = "Pushtree", AnimDetect = "Detect"; //AnimAtack = "Punch"
    #endregion

    #region Input
    private void InputCheck()
    {
        slowWalk = Input.GetKey(KeyCode.LeftShift);
        horizontal = Input.GetAxisRaw(Horzontal);

        if (horizontal != 0) moving = true;
        else moving = false;
    }
    #endregion

    #region Interactive System
    private void InteractiveSystem()
    {
        if (OnGround())
        {
            if (!InteractiveType(interactivechek.jump_))
            {
                if (!InteractiveType(interactivechek.push_))
                {
                    if (!moving)
                    {
                        Idle();
                    }
                    else Move();
                } 
                else Push();
            } 
            else Jump();
        }
    }
    #endregion

    #region Actions
    private void Idle()
    {
        AnimatorController(AnimIdle);
    }

    private void Move()
    {
        if (slowWalk)
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
        }
    }

    private async void WarningAndDetectSatuts()
    {
        if (InteractiveType(interactivechek.detect_))
        {
            AnimatorController(AnimDetect);
            isDetect = true;
            await Task.Yield();
        }

        if (InteractiveType(interactivechek.warning_)) Warning.SetActive(true);
        else Warning.SetActive(false);
    }

    private void Camera()
    {
        vCam.m_Lens.OrthographicSize = camDistance;
    }
    #endregion

    #region CheckPoint
    private void GetPoint()
    {
        if (InteractiveType(interactivechek.Point_)) ckPoint = Transfr();
    }
    #endregion

    #region Direction
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

    #region Animation
    private async void SmootAnimationParameters()
    {
        if (horizontal != 0)
            if (HorizontalAsist < 1)
                HorizontalAsist += SmootAnimationFactor * Time.deltaTime;

        if (horizontal == 0)
            if (HorizontalAsist > 0)
                HorizontalAsist -= SmootAnimationFactor * Time.deltaTime;

        await Task.Yield();
    }

    private async void AnimatorController(string animation)
    {
        animator.SetFloat(AnimPushF, HorizontalAsist);
        animator.SetFloat(AnimRunF, HorizontalAsist);
        animator.SetFloat(AnimWalkF, HorizontalAsist);
        animator.Play(animation);
        await Task.Yield();
    }

    public void ResetDetect()
    {
        transform.position = new Vector2(ckPoint.position.x, transform.position.y);
        vCam.m_Lens.OrthographicSize = 8;
        isDetect = false;
    }
    #endregion

    #region Checkers
    private bool OnGround()
    {
        return Physics.Raycast(transform.position, -transform.up,
            out groundchek.hit, groundchek.distance, groundchek.ground_);
    }
    
    private bool InteractiveType(LayerMask layer)
    {
        var pos = new Vector3(transform.position.x, transform.position.y +
            interactivechek.rayPosition, transform.position.z);

        return Physics.Raycast(pos, transform.forward, 
            out interactivechek.hit, interactivechek.distance, layer);
    }

    private int Type()
    {
        return interactivechek.hit.transform.GetComponent<TypeJump>().typejump;
    }

    private Transform Transfr()
    {
        return interactivechek.hit.transform.GetComponent<Transform>();
    }
    #endregion

    #region Editor mode
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * groundchek.distance);

        var pos = new Vector3(transform.position.x, transform.position.y +
            interactivechek.rayPosition, transform.position.z);

        Gizmos.DrawRay(pos, transform.forward * interactivechek.distance);
    }
#endif
    #endregion
}

#region Check class
[Serializable]
public class GroundChek
{
    [HideInInspector] public RaycastHit hit;
    public LayerMask ground_;
    public float distance;
}

[Serializable]
public class InteractiveChek
{
    [HideInInspector] public RaycastHit hit;
    public LayerMask push_, jump_, 
        warning_, detect_, Point_;
    public float distance, rayPosition;
}
#endregion
