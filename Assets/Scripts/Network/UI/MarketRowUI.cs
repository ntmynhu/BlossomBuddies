using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// One row in the market list. Assign the references in the row prefab.
    /// </summary>
    public class MarketRowUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private Button actionButton;
        [SerializeField] private TMP_Text actionButtonLabel;

        public void Set(string text, string status, string buttonText, bool showButton, UnityAction onClick)
        {
            if (label != null) label.text = text;

            if (statusLabel != null)
            {
                statusLabel.gameObject.SetActive(!string.IsNullOrEmpty(status));
                statusLabel.text = status ?? "";
            }

            if (actionButton != null)
            {
                actionButton.gameObject.SetActive(showButton);
                actionButton.onClick.RemoveAllListeners();
                if (showButton && onClick != null) actionButton.onClick.AddListener(onClick);
            }
            if (actionButtonLabel != null && buttonText != null) actionButtonLabel.text = buttonText;
        }
    }
}
