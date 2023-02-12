using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    private void Awake()
    {
        animator = GetComponent<Animator>();
        checkPoint = GetComponent<CheckPoint>();
        detectSystem = GetComponent<DetectSystem>();
    }

    private void Update()
    {
        SmootAnimationParameters();
    }

    #region variables
    [Header("Animation")]
    [SerializeField][Range(1, 10)] private float smootFactor, smoot;
    private Animator animator;
    private CheckPoint checkPoint;
    private DetectSystem detectSystem;

    private string
        // Parameters
        AnimPushF = "CrouchPush", AnimRunF = "WalkRun",
        AnimWalkF = "IdleWalk";
    #endregion

    #region Animation
    private void SmootAnimationParameters()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal != 0)
        {
            if (smoot < 1)
                smoot += smootFactor * Time.deltaTime;
        }
        else
        {
            if (smoot > 0)
                smoot -= smootFactor * Time.deltaTime;
        }
    }

    public void AnimatorController(string animation)
    {
        animator.SetFloat(AnimPushF, smoot);
        animator.SetFloat(AnimRunF, smoot);
        animator.SetFloat(AnimWalkF, smoot);
        animator.Play(animation);
    }

    public void ResetDetect()
    {
        transform.position = checkPoint.OnCheckPoint;
        detectSystem.IsFail = false;
    }
    #endregion
}
