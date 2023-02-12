using UnityEngine;

public class DetectSystem : MonoBehaviour
{
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (!IsFail) CallMachine();
    }

    #region variables
    private RaycastHit hitInteractive;
    private PlayerController playerController;
    public float gDistance, iDistance, rayIPosition, rayEPosition;

    [SerializeField] private GameObject Warning, Detected;

    [Header("Layers")]
    [SerializeField] private LayerMask jumpL;
    [SerializeField] private LayerMask pushL;
    [SerializeField] private LayerMask pointL;
    [SerializeField] private LayerMask warningL;
    [SerializeField] private LayerMask detectL;
    [SerializeField] private LayerMask tryPushL;
    [SerializeField] private LayerMask groundL;

    readonly private string move = "move", push = "push",
        jump = "jump", warning = "warning", tryPush = "trypush";

    public bool IsFail { get; set; }
    #endregion

    #region Checkers
    private bool OnGround()
    {
        return Physics.Raycast(transform.position, -transform.up, 
            gDistance, groundL);
    }

    private bool IsInteractive(LayerMask l)
    {
        var pos = new Vector3(transform.position.x, transform.position.y +
            rayIPosition, transform.position.z);

        return Physics.Raycast(pos, transform.forward,
            out hitInteractive, iDistance, l);
    }

    private bool Enemy(LayerMask l)
    {
        var pos = new Vector3(transform.position.x, transform.position.y +
            rayEPosition, transform.position.z);

        return Physics.Raycast(pos, transform.up,
            out hitInteractive, iDistance, l);
    }
    #endregion

    #region Call Machine
    private void CallMachine()
    {
        if (OnGround())
        {
            if (!IsInteractive(jumpL))
            {
                if (!IsInteractive(tryPushL))
                {
                    if (!IsInteractive(pushL))
                    {
                        playerController.CallSMachine(move);
                    }
                    else playerController.CallSMachine(push);
                } 
                else playerController.CallSMachine(tryPush);
            }
            else playerController.CallSMachine(jump);
        }

        if (Enemy(detectL))
        {
            playerController.CallSMachine(warning);
            IsFail = true;
        }

        if (Enemy(warningL)) Warning.SetActive(true);
        else Warning.SetActive(false);
    }
    #endregion

    #region Getters
    public bool GetInteractive(LayerMask l)
    {
        return IsInteractive(l);
    }

    public int JumpT
    {
        get { return hitInteractive.transform.GetComponent<TypeJump>().typejump; }
    }
    #endregion

    #region Editor mode
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        var positionI = new Vector3(transform.position.x, transform.position.y +
            rayIPosition, transform.position.z);
        
        var positionE = new Vector3(transform.position.x, transform.position.y +
            rayEPosition, transform.position.z);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * gDistance);
        Gizmos.DrawRay(positionI, transform.forward * iDistance);
        Gizmos.DrawRay(positionE, transform.up * iDistance);
    }
#endif
    #endregion
}
