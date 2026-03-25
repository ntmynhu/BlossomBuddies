using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private Button buyButton;

    private BaseData itemData;

    public void Setup(BaseData data)
    {
        itemData = data;
        itemImage.sprite = data.icon;
        itemPriceText.text = data.buyPrice.ToString();
        ownedText.gameObject.SetActive(false);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyButtonClick);

        if (data.isUnique)
        {
            bool isOwned = InventoryManager.Instance.HasItem(data);
            ownedText.gameObject.SetActive(isOwned);
            itemPriceText.gameObject.SetActive(!isOwned);
            buyButton.interactable = !isOwned;
        }
    }

    private void OnBuyButtonClick()
    {
        ShopManager.Instance.BuyItem(itemData);
    }
}
