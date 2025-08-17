using UnityEditor.Tilemaps;
using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private PlayerAnimation playerAnim;
    [SerializeField] private PlayerMovement playerMovement;

    public Transform ParentTransform => parentTransform;

    private void Update()
    {
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
    }
}
