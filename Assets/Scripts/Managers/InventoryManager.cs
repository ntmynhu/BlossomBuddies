using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlotUI[] furnitureSlots;
    [SerializeField] private InventorySlotUI uiSlotPrefab;
    [SerializeField] private ThirdPersonCameraController thirdPersonCameraController;

    [SerializeField] private List<ScriptableObject> objectDatabase;

    private Dictionary<ScriptableObject, int> inventoryDictionary;

    private void Start()
    {
        InitIventory();
    }

    private void Update()
    {
        HandleFurnitureInventory();
    }

    private void InitIventory()
    {
        inventoryDictionary = new Dictionary<ScriptableObject, int>();

        foreach (var obj in objectDatabase)
        {
            inventoryDictionary[obj] = 0;
        }
    }

    public void AddToInventory(ObjectData objectData)
    {
        if (inventoryDictionary.ContainsKey(objectData))
        {
            inventoryDictionary[objectData]++;
        }
        else
        {
            Debug.LogWarning($"ObjectData {objectData.Name} not found in inventory dictionary.");
        }
    }

    private void HandleFurnitureInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Inventory opened");
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            thirdPersonCameraController.SetMobileController(inventoryPanel.activeSelf);
            GameManager.Instance.Player.SetActive(!inventoryPanel.activeSelf);

            if (inventoryPanel.activeSelf)
            {
                PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.FurnitureState, furnitureSlots[0].ObjectData);
            }
            else
            {
                PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.NormalState, null);
            }
        }

        if (inventoryPanel.activeSelf)
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