using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// In-game HUD shown only while signed in:
    /// - username + logout (top-right),
    /// - Marketplace + Shop buttons (bottom-left),
    /// - Inventory button (bottom-right).
    /// Logging out saves progress, clears the session, and re-runs the load flow so the login
    /// screen shows again (test multiple accounts). Lives on the OnlineCanvas beside MarketPanel.
    /// </summary>
    public class SessionUI : MonoBehaviour
    {
        [Header("Logout / username")]
        [SerializeField] private GameObject logoutRoot;   // shown only when logged in
        [SerializeField] private Button logoutButton;
        [SerializeField] private TMP_Text usernameLabel;

        [Header("HUD buttons")]
        [SerializeField] private GameObject hudRoot;       // shown only when logged in
        [SerializeField] private Button marketButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button inventoryButton;

        private MarketPanel _market;

        private void Awake()
        {
            _market = GetComponent<MarketPanel>();

            if (logoutButton != null) logoutButton.onClick.AddListener(OnLogout);
            if (marketButton != null) marketButton.onClick.AddListener(OnMarket);
            if (shopButton != null) shopButton.onClick.AddListener(OnShop);
            if (inventoryButton != null) inventoryButton.onClick.AddListener(OnInventory);
        }

        private void Start()
        {
            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.OnLoggedIn += Show;
                SessionManager.Instance.OnLoggedOut += Hide;
                SetVisible(SessionManager.Instance.IsLoggedIn);
            }
            else
            {
                SetVisible(false);
            }
        }

        private void OnDestroy()
        {
            if (logoutButton != null) logoutButton.onClick.RemoveListener(OnLogout);
            if (marketButton != null) marketButton.onClick.RemoveListener(OnMarket);
            if (shopButton != null) shopButton.onClick.RemoveListener(OnShop);
            if (inventoryButton != null) inventoryButton.onClick.RemoveListener(OnInventory);
            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.OnLoggedIn -= Show;
                SessionManager.Instance.OnLoggedOut -= Hide;
            }
        }

        private void OnLogout()
        {
            if (DataPersistenceManager.Instance != null) DataPersistenceManager.Instance.SaveGame();
            if (SessionManager.Instance != null) SessionManager.Instance.Logout();
            if (LoadingManager.Instance != null) LoadingManager.Instance.RestartLoadFlow();
        }

        private void OnMarket()
        {
            if (_market != null) _market.Toggle();
        }

        private void OnShop()
        {
            if (ShopManager.Instance != null) ShopManager.Instance.ToggleShop();
        }

        private void OnInventory()
        {
            if (InventoryManager.Instance != null) InventoryManager.Instance.ToggleInventory();
        }

        private void Show() => SetVisible(true);
        private void Hide() => SetVisible(false);

        private void SetVisible(bool visible)
        {
            if (logoutRoot != null) logoutRoot.SetActive(visible);
            if (hudRoot != null) hudRoot.SetActive(visible);

            if (visible && usernameLabel != null && SessionManager.Instance != null)
                usernameLabel.text = SessionManager.Instance.Username;
        }
    }
}
