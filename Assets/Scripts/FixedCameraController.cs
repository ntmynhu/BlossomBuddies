using Unity.Cinemachine;
using UnityEngine;

public class FixedCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Zoom settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoomLens = 40f;
    [SerializeField] private float maxZoomLens = 90f;
    [SerializeField] private float zoomSmoothTime = 0.08f;

    private float targetFov;
    private float fovVelocity;

    private void Start()
    {
        targetFov = cinemachineCamera.Lens.FieldOfView;
    }

    private void Update()
    {
        HandlePCZoom();
    }

    private void HandlePCZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            targetFov -= scroll * zoomSpeed;
            targetFov = Mathf.Clamp(targetFov, minZoomLens, maxZoomLens);
        }

        float current = cinemachineCamera.Lens.FieldOfView;
        float smooth = Mathf.SmoothDamp(current, targetFov, ref fovVelocity, zoomSmoothTime);
        cinemachineCamera.Lens.FieldOfView = smooth;
    }
}
