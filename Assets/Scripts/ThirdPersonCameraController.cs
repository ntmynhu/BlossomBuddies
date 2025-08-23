using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private CinemachineInputAxisController inputAxisController;
    [SerializeField] private float mouseSensitivity = 100f;

    private float inputValueX;
    private float inputValueY;

    private void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        inputAxisController.Controllers[0].Enabled = true;
        inputAxisController.Controllers[1].Enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
#else
                inputAxisController.Controllers[0].Enabled = false;
                inputAxisController.Controllers[1].Enabled = false;
#endif

        /*inputAxisController.Controllers[0].Enabled = false;
        inputAxisController.Controllers[1].Enabled = false;*/

    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
#else
        if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Ignore input if the pointer is over a UI element
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
#endif

        /*if (Input.GetMouseButton(0))
        {
            inputAxisController.Controllers[0].Enabled = true;
            inputAxisController.Controllers[1].Enabled = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            inputAxisController.Controllers[0].Enabled = false;
            inputAxisController.Controllers[1].Enabled = false;
        }*/
    }
}
