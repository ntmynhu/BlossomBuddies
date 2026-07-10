using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// In-game marketplace, driven by scene-authored UI. Put this on an always-active object
    /// (e.g. the Canvas) and assign the references. Toggle with the M key. After any trade it
    /// re-pulls inventory/coins so local state matches the server.
    /// </summary>
    public class MarketPanel : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.M;

        [Header("Root (toggled)")]
        [SerializeField] private GameObject overlayRoot;

        [Header("List")]
        [SerializeField] private Transform listContent;
        [SerializeField] private MarketRowUI rowPrefab;

        [Header("Create form")]
        [SerializeField] private TMP_InputField itemInput;
        [SerializeField] private TMP_InputField qtyInput;
        [SerializeField] private TMP_InputField priceInput;
        [SerializeField] private Button sellButton;

        [Header("Controls")]
        [SerializeField] private Button allTabButton;
        [SerializeField] private Button myTabButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button closeButton;

        [Header("Feedback")]
        [SerializeField] private TMP_Text status;

        private bool _showMine;

        private void Awake()
        {
            if (sellButton != null) sellButton.onClick.AddListener(OnSell);
            if (refreshButton != null) refreshButton.onClick.AddListener(Refresh);
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            if (allTabButton != null) allTabButton.onClick.AddListener(() => { _showMine = false; Refresh(); });
            if (myTabButton != null) myTabButton.onClick.AddListener(() => { _showMine = true; Refresh(); });
        }

        private void Start()
        {
            if (overlayRoot != null) overlayRoot.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                Toggle();
        }

        public void Toggle()
        {
            if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn)
            {
                Debug.Log("[Market] Log in first to open the market.");
                return;
            }
            if (overlayRoot == null) return;

            bool open = !overlayRoot.activeSelf;
            overlayRoot.SetActive(open);
            if (open) Refresh();
        }

        public void Close()
        {
            if (overlayRoot != null) overlayRoot.SetActive(false);
        }

        // ---------- Data ----------

        private void Refresh()
        {
            SetStatus(_showMine ? "Loading your listings..." : "Loading market...");
            if (_showMine)
                GameApi.Market.GetMine(RenderListings, OnError);
            else
                GameApi.Market.GetActive(RenderListings, OnError);
        }

        private void RenderListings(List<MarketListingDto> listings)
        {
            if (listContent == null || rowPrefab == null) return;

            foreach (Transform child in listContent)
                Destroy(child.gameObject);

            foreach (var listing in listings)
                CreateRow(listing);

            SetStatus(listings.Count == 0 ? "No listings." : $"{listings.Count} listing(s).");
        }

        private void CreateRow(MarketListingDto listing)
        {
            var row = Instantiate(rowPrefab, listContent);
            int total = listing.UnitPrice * listing.Quantity;
            string text = $"{listing.ItemDefId}   x{listing.Quantity}   @ {listing.UnitPrice}   (total {total})";

            if (_showMine)
            {
                bool active = listing.Status == "Active";
                row.Set(text, listing.Status, "Cancel", active, () => OnCancel(listing.Id));
            }
            else
            {
                row.Set(text, null, "Buy", true, () => OnBuy(listing.Id));
            }
        }

        // ---------- Actions ----------

        private void OnSell()
        {
            var itemId = itemInput != null ? itemInput.text.Trim() : "";
            if (string.IsNullOrEmpty(itemId)) { SetStatus("Enter an item id."); return; }
            if (!int.TryParse(qtyInput != null ? qtyInput.text : "", out var qty) || qty <= 0) { SetStatus("Enter a valid quantity."); return; }
            if (!int.TryParse(priceInput != null ? priceInput.text : "", out var price) || price <= 0) { SetStatus("Enter a valid price."); return; }

            SetStatus("Posting listing...");
            GameApi.Market.CreateListing(itemId, qty, price,
                listing =>
                {
                    SetStatus($"Listed {listing.Quantity}x {listing.ItemDefId}.");
                    if (itemInput != null) itemInput.text = "";
                    if (qtyInput != null) qtyInput.text = "";
                    if (priceInput != null) priceInput.text = "";
                    SyncAndRefresh();
                },
                OnError);
        }

        private void OnBuy(int listingId)
        {
            SetStatus("Buying...");
            GameApi.Market.Buy(listingId,
                tx => { SetStatus($"Bought for {tx.TotalPrice} coins."); SyncAndRefresh(); },
                OnError);
        }

        private void OnCancel(int listingId)
        {
            SetStatus("Cancelling...");
            GameApi.Market.Cancel(listingId,
                _ => { SetStatus("Listing cancelled."); SyncAndRefresh(); },
                OnError);
        }

        private void SyncAndRefresh()
        {
            if (ServerSyncManager.Instance != null)
                ServerSyncManager.Instance.PullFromServer();
            Refresh();
        }

        private void OnError(ApiError err) => SetStatus("Error: " + err.Message);

        private void SetStatus(string message)
        {
            if (status != null) status.text = message;
        }
    }
}
