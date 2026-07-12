using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// One slot in the common market grid (browse everyone's listings). Shows the seller's
    /// username, the item icon and price. Tapping an on-sale slot buys it. A sold slot shows
    /// who bought it and is not interactable. Self-builds its visuals at runtime.
    /// </summary>
    public class MarketSlotUI : MonoBehaviour
    {
        private Image _background;
        private Image _icon;
        private TMP_Text _sellerLabel;
        private TMP_Text _priceLabel;
        private TMP_Text _footerLabel;   // "BUY" prompt or "Sold to <buyer>"
        private Button _button;

        private static readonly Color ForSaleColor = new Color(0.5098f, 0.4510f, 0.3922f, 1f);
        private static readonly Color SoldColor = new Color(0.45f, 0.55f, 0.55f, 1f);

        public void Init()
        {
            _background = gameObject.GetComponent<Image>();
            if (_background == null) _background = gameObject.AddComponent<Image>();

            _button = gameObject.AddComponent<Button>();
            _button.targetGraphic = _background;

            _sellerLabel = UIFactory.Text(transform, "", 16, TextAlignmentOptions.Center, FontStyles.Bold);
            _sellerLabel.color = Color.white;
            _sellerLabel.raycastTarget = false;
            Anchor(_sellerLabel.rectTransform, new Vector2(0.05f, 0.82f), new Vector2(0.95f, 1f));

            _icon = UIFactory.Image(transform, "Icon", Color.white);
            _icon.preserveAspect = true;
            _icon.raycastTarget = false;
            Anchor(_icon.rectTransform, new Vector2(0.2f, 0.42f), new Vector2(0.8f, 0.82f));

            _priceLabel = UIFactory.Text(transform, "", 18, TextAlignmentOptions.Center, FontStyles.Bold);
            _priceLabel.color = Color.white;
            _priceLabel.raycastTarget = false;
            Anchor(_priceLabel.rectTransform, new Vector2(0.05f, 0.22f), new Vector2(0.95f, 0.42f));

            _footerLabel = UIFactory.Text(transform, "", 16, TextAlignmentOptions.Center);
            _footerLabel.color = Color.white;
            _footerLabel.raycastTarget = false;
            Anchor(_footerLabel.rectTransform, new Vector2(0.05f, 0f), new Vector2(0.95f, 0.22f));
        }

        public void ShowForSale(Sprite icon, string seller, int quantity, int unitPrice, UnityAction onBuy)
        {
            _background.color = ForSaleColor;
            _sellerLabel.text = seller;
            SetIcon(icon);
            _priceLabel.text = quantity > 1 ? $"x{quantity}  @ {unitPrice}" : $"{unitPrice}";
            _footerLabel.text = $"BUY  ({unitPrice * quantity})";
            _button.interactable = true;
            _button.onClick.RemoveAllListeners();
            if (onBuy != null) _button.onClick.AddListener(onBuy);
        }

        public void ShowSold(Sprite icon, string seller, string buyer, int quantity, int unitPrice)
        {
            _background.color = SoldColor;
            _sellerLabel.text = seller;
            SetIcon(icon);
            _priceLabel.text = quantity > 1 ? $"x{quantity}  @ {unitPrice}" : $"{unitPrice}";
            _footerLabel.text = string.IsNullOrEmpty(buyer) ? "SOLD" : $"Sold to {buyer}";
            _button.interactable = false;
            _button.onClick.RemoveAllListeners();
        }

        private void SetIcon(Sprite icon)
        {
            _icon.gameObject.SetActive(icon != null);
            if (icon != null) _icon.sprite = icon;
        }

        private static void Anchor(RectTransform rt, Vector2 min, Vector2 max)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
    }
}
