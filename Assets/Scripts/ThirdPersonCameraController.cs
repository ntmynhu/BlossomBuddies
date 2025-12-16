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
    [SerializeField] private float minZoomDistance = 2f;
    [SerializeField] private float maxZoomDistance = 12f;

    private bool isMobileController = false;

    private void Start()
    {
        SetMobileController(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMobileController(!isMobileController);
        }

        if (!isMobileController) return;

        // Zoom logic for PC only
        if (!isMobileController && orbitalFollow != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                //orbitalFollow.CameraDistance -= scroll * zoomSpeed;
                //orbitalFollow.CameraDistance = Mathf.Clamp(orbitalFollow.CameraDistance, minZoomDistance, maxZoomDistance);
            }
        }

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
}
