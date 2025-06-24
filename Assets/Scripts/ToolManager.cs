using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private ToolInfo[] toolInfos;

    private Tool currentTool;
    private List<Tool> toolList = new();

    #region Singleton
    private static ToolManager instance;
    public static ToolManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public void InitializeTools(Transform parentTransform)
    {
        foreach (var toolInfo in toolInfos)
        {
            GameObject toolObject = Instantiate(toolInfo.toolPrefab, parentTransform);
            toolObject.SetActive(false);

            Tool tool = toolObject.GetComponent<Tool>();
            tool.toolInfo = toolInfo;

            toolList.Add(tool);

            Debug.Log($"Initialized tool: {toolInfo.toolName} , {tool}");
        }
    }

    public void SetCurrentTool(Tool newTool)
    {
        if (currentTool != null)
        {
            currentTool.gameObject.SetActive(false);
        }

        currentTool = newTool;
        if (currentTool != null)
        {
            currentTool.gameObject.SetActive(true);
        }
    }

    public void TestTool(int index)
    {
        SetCurrentTool(toolList[index]);
    }
}
