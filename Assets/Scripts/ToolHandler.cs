using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;

    public Transform ParentTransform => parentTransform;

    private void Start()
    {
        //ToolManager.Instance.InitializeTools(parentTransform);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    ToolManager.Instance.TestTool(0);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    ToolManager.Instance.TestTool(1);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    ToolManager.Instance.TestTool(2);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    ToolManager.Instance.SetCurrentTool(null); // Deselect current tool
        //}    
    }
}
