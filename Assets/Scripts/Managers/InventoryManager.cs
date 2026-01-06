using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private GameObject furnitureInventoryPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlotUI uiSlotPrefab;
    [SerializeField] private ThirdPersonCameraController thirdPersonCameraController;

    [SerializeField] private List<ToolInfo> gardenToolDatabase;
    [SerializeField] private List<ScriptableObject> furnitureDatabase;

    private Dictionary<ToolInfo, int> inventoryDictionary;

    public bool IsInitialized => inventoryDictionary != null;
    public bool IsInventoryOpen => inventoryPanel.activeSelf;

    private void Start()
    {
        InitIventory();
    }

    private void Update()
    {
        HandleFurnitureInventory();
        HandleGardenToolInventory();
    }

    private void InitIventory()
    {
        inventoryDictionary = new Dictionary<ToolInfo, int>();

        foreach (var obj in gardenToolDatabase)
        {
            inventoryDictionary[obj] = 0;
        }
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in inventoryDictionary)
        {
            if (item.Value > 0)
            {
                InventorySlotUI slot = Instantiate(uiSlotPrefab, inventoryPanel.transform);
                slot.SetData(item.Key);
            }
        }
    }

    public void OnItemSelected(ToolInfo item)
    {
        Debug.Log($"Item selected: {item.name}");
        GameManager.Instance.ToolHandler.SelectTool(item);
    }

    public void AddToInventory(ToolInfo objectData)
    {
        if (inventoryDictionary.ContainsKey(objectData))
        {
            inventoryDictionary[objectData]++;
            Debug.Log($"Added {objectData.name} to inventory. New quantity: {inventoryDictionary[objectData]}");

            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"ObjectData {objectData.name} not found in inventory dictionary.");
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
}

[Serializable]
public class InventoryItem
{
    public ScriptableObject item;
    public int quantity;
}