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

    [SerializeField] private List<PreviewData> defaultObjectDatabase;
    [SerializeField] private List<PreviewData> objectDatabase;
    [SerializeField] private List<ScriptableObject> furnitureDatabase;

    private Dictionary<PreviewData, int> inventoryDictionary;

    public bool IsInitialized => inventoryDictionary != null;
    public bool IsInventoryOpen => inventoryPanel.activeSelf;

    private void Start()
    {
    }

    private void Update()
    {
        HandleFurnitureInventory();
        HandleGardenToolInventory();
    }

    private void InitIventory()
    {
        inventoryDictionary = new Dictionary<PreviewData, int>();

        foreach (var obj in defaultObjectDatabase)
        {
            inventoryDictionary[obj] = 1;
        }

        Debug.Log("Inventory initialized with " + inventoryDictionary.Count + " items.");
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Updating inventory UI with " + inventoryDictionary.Count + " items.");

        foreach (var item in inventoryDictionary)
        {
            if (item.Value > 0)
            {
                InventorySlotUI slot = Instantiate(uiSlotPrefab, inventoryContent.transform);
                slot.SetData(item.Key, inventoryDictionary[item.Key]);
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
        AddItemToDictionary(inventoryDictionary, objectData);
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
            Debug.Log($"Key {key.name} not found in inventory dictionary. Adding it with quantity 1.");
            dict[key] = 1;
        }
    }

    private void HandleGardenToolInventory()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Inventory opened");
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            GameManager.Instance.SetCameraFrozen(inventoryPanel.activeSelf);
        }

        if (inventoryPanel.activeSelf)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                inventoryPanel.SetActive(false);
                GameManager.Instance.SetCameraFrozen(false);
            }
        }
    }

    private void HandleFurnitureInventory()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Inventory opened");
            furnitureInventoryPanel.SetActive(!furnitureInventoryPanel.activeSelf);
            GameManager.Instance.SetCameraFrozen(furnitureInventoryPanel.activeSelf);

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

    public List<T> GetInventoryObjectsByType<T>() where T : ScriptableObject
    {
        List<T> result = new List<T>();

        foreach (var item in inventoryDictionary)
        {
            if (item.Key is T typedKey && item.Value > 0)
            {
                result.Add(typedKey);
            }
        }

        return result;
    }

    public int GetItemQuantity(PreviewData objectData)
    {
        if (inventoryDictionary.TryGetValue(objectData, out int quantity))
        {
            return quantity;
        }
        return 0;
    }

    public void LoadData(GameData data)
    {
        if (data.inventoryDataList != null && data.inventoryDataList.Count > 0)
        {
            List<InventoryData> loadedItems = data.inventoryDataList;

            inventoryDictionary = new Dictionary<PreviewData, int>();

            foreach (var item in loadedItems)
            {
                PreviewData objectData = defaultObjectDatabase.Find(obj => obj.Id == item.objectId);

                if (objectData == null)
                {
                    objectData = objectDatabase.Find(obj => obj.Id == item.objectId);
                }

                if (objectData != null)
                {
                    inventoryDictionary[objectData] = item.quantity;
                }
                else
                {
                    Debug.LogWarning($"ObjectData with ID {item.objectId} not found in object database.");
                }
            }

            Debug.Log("Inventory loaded successfully with " + inventoryDictionary.Count + " items.");
        }
        else
        {
            InitIventory();
        }

        List<ToolInfo> toolList = GetInventoryObjectsByType<ToolInfo>();
        StartCoroutine(ToolManager.Instance.InitializeTools(toolList));

        UpdateInventoryUI();
    }

    public void SaveData(ref GameData data)
    {
        foreach (var item in inventoryDictionary)
        {
            if (item.Value > 0)
            {
                InventoryData inventoryData = new InventoryData
                {
                    objectId = item.Key.Id,
                    quantity = item.Value
                };

                data.inventoryDataList.Add(inventoryData);
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