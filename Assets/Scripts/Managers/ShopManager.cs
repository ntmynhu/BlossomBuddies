using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{
    [Header("Shop UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private ShopItemUI itemPrefab;

    [Header("Shop Data")]
    [SerializeField] private List<ObjectsDatabaseSO> objectsDatabase;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleShop();
        }
    }

    private void PopulateShop()
    {
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var database in objectsDatabase)
        {
            foreach (var itemData in database.objectDatas)
            {
                if (itemData.isSellable)
                {
                    var itemUI = Instantiate(itemPrefab, contentPanel.transform);
                    itemUI.Setup(itemData);
                }
            }
        }
    }

    public void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
        GameManager.Instance.SetCameraFrozen(shopPanel.activeSelf);

        if (shopPanel.activeSelf)
        {
            PopulateShop();
        }
    }

    public void BuyItem(BaseData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("Attempted to buy a null item.");
            return;
        }

        InventoryManager.Instance.AddItem(itemData);
        Debug.Log("Bought item: " + itemData.name);

        // If the item is unique, refresh the shop to update availability
        if (itemData.isUnique)
        {
            PopulateShop();
        }
    }
}