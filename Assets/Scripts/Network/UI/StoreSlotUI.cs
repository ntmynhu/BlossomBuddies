using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// One slot in the player's store (Hay Day "Tom's Store" style). A slot holds at most
    /// one listing and has three visual states: empty ("Create New Trade"), active (item on
    /// sale, tap to cancel) and sold (tap to collect the coins). Builds its own visuals at
    /// runtime so no prefab or scene wiring is needed — just Instantiate the component's
    /// GameObject and call Init().
    /// </summary>
    public class StoreSlotUI : MonoBehaviour
    {
        private Image _background;
        private Image _icon;
        private TMP_Text _label;      // price when active, "Create New Trade" when empty
        private GameObject _soldBadge;
        private Button _button;

        private static readonly Color EmptyColor = new Color(0.80f, 0.72f, 0.55f, 1f);
        private static readonly Color ActiveColor = new Color(0.62f, 0.47f, 0.32f, 1f);
        private static readonly Color SoldColor = new Color(0.45f, 0.70f, 0.40f, 1f);

        public void Init()
        {
            _background = gameObject.GetComponent<Image>();
            if (_background == null) _background = gameObject.AddComponent<Image>();

            _button = gameObject.AddComponent<Button>();
            _button.targetGraphic = _background;

            // Item icon (centered, hidden until an item is placed).
            _icon = UIFactory.Image(transform, "Icon", Color.white);
            _icon.preserveAspect = true;
            _icon.raycastTarget = false;
            var iconRt = _icon.rectTransform;
            iconRt.anchorMin = new Vector2(0.15f, 0.28f);
            iconRt.anchorMax = new Vector2(0.85f, 0.9f);
            iconRt.offsetMin = iconRt.offsetMax = Vector2.zero;

            // Bottom label: price or the "Create New Trade" prompt.
            _label = UIFactory.Text(transform, "", 20, TextAlignmentOptions.Center, FontStyles.Bold);
            _label.color = Color.white;
            _label.enableWordWrapping = true;
            var labelRt = _label.rectTransform;
            labelRt.anchorMin = new Vector2(0.05f, 0f);
            labelRt.anchorMax = new Vector2(0.95f, 0.28f);
            labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;

            // "Sold" badge overlay.
            _soldBadge = UIFactory.Text(transform, "SOLD", 22, TextAlignmentOptions.Center, FontStyles.Bold).gameObject;
            var badgeText = _soldBadge.GetComponent<TMP_Text>();
            badgeText.color = new Color(0.15f, 0.35f, 0.1f);
            var badgeRt = badgeText.rectTransform;
            badgeRt.anchorMin = new Vector2(0.1f, 0.55f);
            badgeRt.anchorMax = new Vector2(0.9f, 0.95f);
            badgeRt.offsetMin = badgeRt.offsetMax = Vector2.zero;
        }

        public void ShowEmpty(UnityAction onCreate)
        {
            _background.color = EmptyColor;
            _icon.gameObject.SetActive(false);
            _soldBadge.SetActive(false);
            _label.text = "Create\nNew Trade";
            Wire(onCreate);
        }

        public void ShowActive(Sprite icon, int quantity, int unitPrice, UnityAction onCancel)
        {
            _background.color = ActiveColor;
            SetIcon(icon);
            _soldBadge.SetActive(false);
            _label.text = quantity > 1 ? $"x{quantity}  -{unitPrice}" : $"-{unitPrice}";
            Wire(onCancel);
        }

        public void ShowSold(Sprite icon, int quantity, int unitPrice, UnityAction onCollect)
        {
            _background.color = SoldColor;
            SetIcon(icon);
            _soldBadge.SetActive(true);
            _label.text = $"+{unitPrice * quantity}";
            Wire(onCollect);
        }

        private void SetIcon(Sprite icon)
        {
            _icon.gameObject.SetActive(icon != null);
            if (icon != null) _icon.sprite = icon;
        }

        private void Wire(UnityAction onClick)
        {
            _button.onClick.RemoveAllListeners();
            if (onClick != null) _button.onClick.AddListener(onClick);
        }
    }
}
