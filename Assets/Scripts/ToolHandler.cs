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
                tool.UseTool(playerAnim, playerMovement);
            }
        }
    }

    public void SelectTool(int toolIndex)
    {
        ToolManager.Instance.TestTool(toolIndex, parentTransform);
    }

    public void UnSelectTool()
    {
        ToolManager.Instance.SetCurrentTool(null, parentTransform);
    }
}
