using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private CinemachineInputAxisController inputAxisController;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Mobile preset")]
    [SerializeField] private float mobileSensitivityX = 600f;
    [SerializeField] private float mobileSensitivityY = -500f;

    [Header("PC preset")]
    [SerializeField] private float pcSensitivityX = 300f;
    [SerializeField] private float pcSensitivityY = -250f;

    [Header("Zoom settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoomLens = 40f;
    [SerializeField] private float maxZoomLens = 90f;
    [SerializeField] private float zoomSmoothTime = 0.08f;

    private float targetFov;
    private float fovVelocity;

    private bool isMobileController = false;

    private void Start()
    {
        targetFov = cinemachineCamera.Lens.FieldOfView;
        SetMobileController(false);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    SetMobileController(!isMobileController);
        //}

        // Zoom logic for PC only
        if (!isMobileController && orbitalFollow != null)
        {
            HandlePCZoom();
        }

        if (!isMobileController) return;

        if (Input.GetMouseButton(0))
        {
            inputAxisController.Controllers[0].Enabled = true;
            inputAxisController.Controllers[1].Enabled = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            inputAxisController.Controllers[0].Enabled = false;
            inputAxisController.Controllers[1].Enabled = false;
        }
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

    public void SetMobileController(bool isMobile)
    {
        isMobileController = isMobile;

        if (isMobileController)
        {
            inputAxisController.Controllers[0].Enabled = false;
            inputAxisController.Controllers[1].Enabled = false;

            inputAxisController.Controllers[0].Input.LegacyGain = mobileSensitivityX;
            inputAxisController.Controllers[1].Input.LegacyGain = mobileSensitivityY;

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            inputAxisController.Controllers[0].Enabled = true;
            inputAxisController.Controllers[1].Enabled = true;

            inputAxisController.Controllers[0].Input.LegacyGain = pcSensitivityX;
            inputAxisController.Controllers[1].Input.LegacyGain = pcSensitivityY;

            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetCameraFrozen(bool value)
    {
        cinemachineCamera.gameObject.SetActive(!value);
    }
}
