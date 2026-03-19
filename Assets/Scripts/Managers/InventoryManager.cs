using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : Singleton<InventoryManager>, IDataPersistence
{
    [SerializeField] private GameObject furnitureInventoryPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject inventoryContent;
    [SerializeField] private InventorySlotUI uiSlotPrefab;
    [SerializeField] private ThirdPersonCameraController thirdPersonCameraController;

    [SerializeField] private List<ToolInfo> gardenToolDatabase;
    [SerializeField] private List<PlantData> plantDatabase;
    [SerializeField] private List<ScriptableObject> furnitureDatabase;

    private Dictionary<ToolInfo, int> toolInventoryDictionary;
    private Dictionary<PlantData, int> plantInventoryDictionary;

    public bool IsInitialized => toolInventoryDictionary != null;
    public bool IsInventoryOpen => inventoryPanel.activeSelf;

    private void Start()
    {
        InitToolIventory();
        InitPlantInventory();
    }

    private void Update()
    {
        HandleFurnitureInventory();
        HandleGardenToolInventory();
    }

    private void InitToolIventory()
    {
        toolInventoryDictionary = new Dictionary<ToolInfo, int>();

        foreach (var obj in gardenToolDatabase)
        {
            toolInventoryDictionary[obj] = 0;
        }
    }

    private void InitPlantInventory()
    {
        plantInventoryDictionary = new Dictionary<PlantData, int>();

        foreach (var obj in plantDatabase)
        {
            plantInventoryDictionary[obj] = 0;
        }
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in toolInventoryDictionary)
        {
            if (item.Value > 0)
            {
                InventorySlotUI slot = Instantiate(uiSlotPrefab, inventoryContent.transform);
                slot.SetData(item.Key, toolInventoryDictionary[item.Key]);
            }
        }

        foreach (var item in plantInventoryDictionary)
        {
            if (item.Value > 0)
            {
                InventorySlotUI slot = Instantiate(uiSlotPrefab, inventoryContent.transform);
                slot.SetData(item.Key, plantInventoryDictionary[item.Key]);
            }
        }
    }

    public void OnItemSelected(PreviewData item)
    {
        Debug.Log($"Item selected: {item.name}");

        if (item is ToolInfo toolInfo)
        {
            GameManager.Instance.ToolHandler.SelectTool(toolInfo);
        }
    }

    public void AddToInventory(PreviewData objectData)
    {
        switch (objectData)
        {
            case ToolInfo tool:
                AddItemToDictionary(toolInventoryDictionary, tool);
                break;

            case PlantData plant:
                AddItemToDictionary(plantInventoryDictionary, plant);
                break;

            default:
                Debug.LogWarning($"Unsupported object type: {objectData.GetType().Name}");
                break;
        }
    }

    private void AddItemToDictionary<T>(Dictionary<T, int> dict, T key) where T : ScriptableObject
    {
        if (dict.ContainsKey(key))
        {
            dict[key]++;
            Debug.Log($"Added {key.name} to inventory. New quantity: {dict[key]}");

            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"ObjectData {key.name} not found in inventory dictionary.");
        }
    }

    private void HandleGardenToolInventory()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Inventory opened");
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            thirdPersonCameraController.SetMobileController(inventoryPanel.activeSelf);
            thirdPersonCameraController.SetCameraFrozen(inventoryPanel.activeSelf);
            GameManager.Instance.PlayerMovement.SetMovementEnable(!inventoryPanel.activeSelf);
        }

        if (inventoryPanel.activeSelf)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                inventoryPanel.SetActive(false);
                thirdPersonCameraController.SetMobileController(false);
                thirdPersonCameraController.SetCameraFrozen(false);
                GameManager.Instance.PlayerMovement.SetMovementEnable(true);
            }
        }
    }

    private void HandleFurnitureInventory()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Inventory opened");
            furnitureInventoryPanel.SetActive(!furnitureInventoryPanel.activeSelf);
            thirdPersonCameraController.SetMobileController(furnitureInventoryPanel.activeSelf);
            GameManager.Instance.Player.SetActive(!furnitureInventoryPanel.activeSelf);

            if (furnitureInventoryPanel.activeSelf)
            {   
                PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.FurnitureState, furnitureDatabase[0] as ObjectData);
            }
            else
            {
                if (ToolManager.Instance.GetCurrentTool() == null)
                {
                    PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.NormalState, null);
                }
                else
                {
                    GameManager.Instance.ToolHandler.SelectTool(ToolManager.Instance.GetCurrentTool().ToolInfo);
                }
            }
        }

        if (furnitureInventoryPanel.activeSelf)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                PlacementSystem.Instance.TriggerAction();
            }
        }
    }

    public void LoadData(GameData data)
    {
        
    }

    public void SaveData(ref GameData data)
    {
        
    }
}

[Serializable]
public class InventoryItem
{
    public ScriptableObject item;
    public int quantity;
}