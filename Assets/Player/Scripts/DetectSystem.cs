using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class DetectSystem : MonoBehaviour
{
    private void FixedUpdate()
    {
        CallMachine();
    }

    #region variables
    [Header("Raycast configuration")]
    [SerializeField] private float gDistance;
    [SerializeField] private float iDistance;
    [SerializeField] private float rayPosition;
    private RaycastHit hitInteractive;

    [Header("Warning/Detect gameobject")]
    [SerializeField] private GameObject WarningGm;
    [SerializeField] private GameObject Detected;

    [SerializeField] private Layers layers;
    [SerializeField] private Callers callers;

    public bool IsFail { get; set; }
    #endregion

    #region Checkers
    private bool OnGroundCheck()
    {
        return Physics.Raycast(transform.position, -transform.up, 
            gDistance, layers.ground);
    }

    private bool InteractionCheck(LayerMask l)
    {
        var pos = new Vector3(transform.position.x, transform.position.y +
            rayPosition, transform.position.z);

        return Physics.Raycast(pos, transform.forward,
            out hitInteractive, iDistance, l);
    }
    #endregion

    #region Call Machine
    private void CallMachine()
    {
        if (!IsFail)
        {
            if (OnGroundCheck())
            {
                if (!InteractionCheck(layers.jump))
                {
                    if (!InteractionCheck(layers.tryPush))
                    {
                        if (!InteractionCheck(layers.push))
                        {
                            callers.Move?.Invoke();
                        }
                        else callers.Push?.Invoke();
                    }
                    else callers.Trypush?.Invoke();
                }
                else callers.Jump?.Invoke(JumpT());
            }
            else callers.Gravity?.Invoke();

            // CAMBIAR sistema de deteccion
            if (InteractionCheck(layers.detect))
            {
                callers.Warnings?.Invoke();
                IsFail = true;
            }

            if (InteractionCheck(layers.warning)) WarningGm.SetActive(true);
            else WarningGm.SetActive(false);
        }
    }
    #endregion

    #region Getters
    public bool GetInteractive(LayerMask l)
    {
        return InteractionCheck(l);
    }

    private int JumpT()
    {
        return hitInteractive.transform.GetComponent<TypeJump>().typejump;
    }
    #endregion

    #region Editor mode
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        var positionI = new Vector3(transform.position.x, transform.position.y +
            rayPosition, transform.position.z);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * gDistance);
        Gizmos.DrawRay(positionI, transform.forward * iDistance);
    }
#endif
    #endregion
}

#region Extend class
[Serializable]
public class Callers
{
    public UnityEvent Move;
    public UnityEvent<int> Jump;
    public UnityEvent Push;
    public UnityEvent Trypush;
    public UnityEvent Warnings;
    public UnityEvent Gravity;
}

[Serializable]
public class Layers
{
    public LayerMask jump;
    public LayerMask push;
    public LayerMask point;
    public LayerMask warning;
    public LayerMask detect;
    public LayerMask tryPush;
    public LayerMask ground;
}
#endregion
