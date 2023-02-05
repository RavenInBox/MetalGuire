using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CamEffect : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float camDistance;

    [Header("Screen")]
    [SerializeField] private Image img;
    [HideInInspector] public float imgAlph;

    private void Camera()
    {
        vCam.m_Lens.OrthographicSize = camDistance;

        var tempColor = img.color;
        tempColor.a = imgAlph;
        img.color = tempColor;
    }

    
    void Update()
    {
        Camera();
    }
}
