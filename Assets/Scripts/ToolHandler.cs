using UnityEditor.Tilemaps;
using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Transform petTransform;
    [SerializeField] private GameObject player;

    private PetStateHandler currentPet = null;
    private GameObject currentInteraction;

    public Transform ParentTransform => parentTransform;
    public PetStateHandler CurrentPet => currentPet;
    public GameObject CurrentInteraction => currentInteraction;

    private void Start()
    {
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    SelectTool(0);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    SelectTool(1);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    SelectTool(2);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    SelectTool(3);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    SelectTool(4);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    SelectTool(5);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    SelectTool(6);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    SelectTool(7);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    SelectTool(8);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    UnSelectTool();
        //}

        if (Input.GetMouseButtonDown(0))
        {
            Tool tool = ToolManager.Instance.GetCurrentTool();
            if (tool != null && !InventoryManager.Instance.IsInventoryOpen)
            {
                tool.UseTool();
            }
        }
    }

    public void OnPickupPet(PetStateHandler pet)
    {
        pet.NavMeshAgent.enabled = false;
        pet.Rigidbody.isKinematic = true;
        pet.transform.SetParent(petTransform);

        pet.transform.localPosition = Vector3.zero;
        pet.transform.localRotation = Quaternion.identity;

        currentPet = pet;
    }

    public void OnPutDownPet(PetStateHandler pet)
    {
        Vector3 targetPos = parentTransform.position + parentTransform.forward;

        pet.transform.position = targetPos;
        pet.transform.SetParent(null);

        pet.NavMeshAgent.enabled = true;
        pet.Rigidbody.isKinematic = false;

        currentPet = null;
    }

    public void SelectTool(ToolInfo toolInfo)
    {
        Tool newTool = ToolManager.Instance.TestTool(toolInfo, parentTransform);

        if (newTool != null)
        {
            newTool.OnToolSelected(player);
        }
        else
        {
            Debug.LogWarning("No tool found at index: " + toolInfo.name);
        }
    }

    public void UnSelectTool()
    {
        ToolManager.Instance.SetCurrentTool(null, parentTransform);
        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.NormalState, null);
    }

    public void SetCurrentInteraction(GameObject interaction)
    {
        currentInteraction = interaction;
    }
}
