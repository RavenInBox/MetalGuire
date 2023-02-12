using Cinemachine;
using UnityEngine;

public class CameraAssist : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float camDistance;

    private void cameraChange()
    {
        vCam.m_Lens.OrthographicSize = camDistance;
    }

    public void CameraReset()
    {
        vCam.m_Lens.OrthographicSize = 8;
    }
}
