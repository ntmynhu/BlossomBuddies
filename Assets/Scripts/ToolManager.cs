using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : Singleton<ToolManager>
{
    [SerializeField] private ToolInfo[] toolInfos;

    private Tool currentTool;

    private List<Tool> toolList = new();

    private Transform initialTransform;

    public IEnumerator InitializeTools(Transform spawnTransform)
    {
        initialTransform = spawnTransform;

        foreach (var toolInfo in toolInfos)
        {
            GameObject toolObject = Instantiate(toolInfo.toolPrefab, spawnTransform);

            Tool tool = toolObject.GetComponent<Tool>();
            tool.toolInfo = toolInfo;

            toolList.Add(tool);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetCurrentTool(Tool newTool, Transform parent)
    {
        if (currentTool != null)
        {
            currentTool.transform.SetParent(initialTransform);
            currentTool.transform.localPosition = Vector3.zero;

            Rigidbody oldRb = currentTool.GetComponent<Rigidbody>();
            oldRb.useGravity = true;
            oldRb.isKinematic = false;
        }

        currentTool = newTool;
        if (currentTool != null)
        {
            Rigidbody rb = currentTool.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            currentTool.transform.SetParent(parent);

            currentTool.transform.localPosition = currentTool.initialPos;
            currentTool.transform.localRotation = currentTool.initialRot;
        }

        PlacementSystem.Instance.ShowIndicator(currentTool);
    }

    public void TestTool(int index, Transform parent)
    {
        SetCurrentTool(toolList[index], parent);
    }

    public Tool GetCurrentTool()
    {
        return currentTool;
    }
}
