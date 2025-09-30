using UnityEditor.Tilemaps;
using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Transform petTransform;
    [SerializeField] private PlayerAnimation playerAnim;
    [SerializeField] private PlayerMovement playerMovement;

    public Transform ParentTransform => parentTransform;
    public Transform PetTransform => petTransform;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectTool(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectTool(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectTool(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectTool(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectTool(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectTool(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectTool(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectTool(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            UnSelectTool();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Tool tool = ToolManager.Instance.GetCurrentTool();
            if (tool != null)
            {
                tool.UseTool();
            }
        }
    }

    public void SelectTool(int toolIndex)
    {
        Tool newTool = ToolManager.Instance.TestTool(toolIndex, parentTransform);

        if (newTool != null)
        {
            newTool.OnToolSelected(playerAnim, playerMovement);
        }
        else
        {
            Debug.LogWarning("No tool found at index: " + toolIndex);
        }
    }

    public void UnSelectTool()
    {
        ToolManager.Instance.SetCurrentTool(null, parentTransform);
        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.NormalState, null);
    }
}
