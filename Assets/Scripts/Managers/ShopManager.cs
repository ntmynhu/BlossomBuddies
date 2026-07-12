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
        bool willOpen = !shopPanel.activeSelf;

        // Only one panel at a time: ignore the open request if another panel is up.
        if (willOpen && GameUIState.AnyOtherOpen(UIPanel.Shop)) return;

        shopPanel.SetActive(willOpen);
        GameUIState.ShopOpen = willOpen;
        GameManager.Instance.SetMovementFrozen(willOpen);

        if (willOpen)
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

        if (GameManager.Instance.CurrentHeart < itemData.buyPrice)
        {
            Debug.Log("Not enough hearts to buy: " + itemData.name);
            return;
        }

        GameManager.Instance.CurrentHeart -= itemData.buyPrice;
        InventoryManager.Instance.AddItem(itemData);
        Debug.Log("Bought item: " + itemData.name);

        if (itemData.isUnique)
        {
            PopulateShop();
        }
    }
}