using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private LayerMask pointL;
    private DetectSystem detectSystem;
    private Vector3 resetPoint;

    private void Awake()
    {
        detectSystem = GetComponent<DetectSystem>();
    }

    void Update()
    {
        GetPoint();
    }

    #region CheckPoint
    private void GetPoint()
    {
        if (detectSystem.GetInteractive(pointL))
            OnCheckPoint = new Vector3(transform.position.x,
                transform.position.y, transform.position.z);
    }
    #endregion

    public Vector3 OnCheckPoint
    {
        get { return resetPoint; }
        set { resetPoint = value; }
    }
}
