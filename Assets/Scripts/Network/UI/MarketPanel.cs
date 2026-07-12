using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// In-game marketplace (Hay Day "Tom's Store" style), driven by scene-authored UI.
    /// Two tabs:
    ///   * Market  — every player's active listings; tap Buy to purchase.
    ///   * My Store — a grid of slots you own. An empty slot opens the sell picker; an active
    ///                slot can be cancelled; a sold slot is tapped to collect the proceeds.
    /// Put this on an always-active object (e.g. the Canvas) and assign the references.
    /// Toggle with the M key. After any trade it re-pulls inventory/coins from the server.
    /// </summary>
    public class MarketPanel : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.M;
        [SerializeField] private int storeSlotCount = 8;

        [Header("Root (toggled)")]
        [SerializeField] private GameObject overlayRoot;

        [Header("Tabs")]
        [SerializeField] private Button marketTabButton;
        [SerializeField] private Button storeTabButton;
        [SerializeField] private GameObject marketTabRoot;
        [SerializeField] private GameObject storeTabRoot;

        [Header("Market tab (browse all)")]
        [SerializeField] private Transform marketGridContent;

        [Header("My Store tab (slots)")]
        [SerializeField] private Transform storeGridContent;

        [Header("Sell picker")]
        [SerializeField] private GameObject pickerRoot;
        [SerializeField] private Transform pickerListContent;
        [SerializeField] private TMP_Text pickerSelectedLabel;
        [SerializeField] private TMP_InputField pickerQtyInput;
        [SerializeField] private TMP_InputField pickerPriceInput;
        [SerializeField] private Button pickerConfirmButton;
        [SerializeField] private Button pickerCancelButton;

        [Header("Controls")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button closeButton;

        [Header("Feedback")]
        [SerializeField] private TMP_Text status;

        private bool _showMine;
        private readonly List<StoreSlotUI> _slots = new List<StoreSlotUI>();
        private BaseData _pickerSelected;

        private void Awake()
        {
            if (refreshButton != null) refreshButton.onClick.AddListener(Refresh);
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            if (marketTabButton != null) marketTabButton.onClick.AddListener(() => ShowTab(false));
            if (storeTabButton != null) storeTabButton.onClick.AddListener(() => ShowTab(true));
            if (pickerConfirmButton != null) pickerConfirmButton.onClick.AddListener(OnPickerConfirm);
            if (pickerCancelButton != null) pickerCancelButton.onClick.AddListener(ClosePicker);
        }

        private void Start()
        {
            if (overlayRoot != null) overlayRoot.SetActive(false);
            if (pickerRoot != null) pickerRoot.SetActive(false);
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

            // Only one panel at a time: ignore the open request if another panel is up.
            if (open && GameUIState.AnyOtherOpen(UIPanel.Market)) return;

            overlayRoot.SetActive(open);
            GameUIState.MarketOpen = open;
            if (GameManager.Instance != null) GameManager.Instance.SetMovementFrozen(open);

            if (open)
            {
                // Push local hearts/coins to the server first so the server-side balance
                // matches what the player sees before any purchase is attempted.
                if (ServerSyncManager.Instance != null)
                    ServerSyncManager.Instance.PushToServer();
                ClosePicker();
                ShowTab(_showMine);
            }
        }

        public void Close()
        {
            ClosePicker();
            if (overlayRoot != null) overlayRoot.SetActive(false);
            GameUIState.MarketOpen = false;
            if (GameManager.Instance != null) GameManager.Instance.SetMovementFrozen(false);
        }

        // ---------- Tabs ----------

        private void ShowTab(bool mine)
        {
            _showMine = mine;
            if (marketTabRoot != null) marketTabRoot.SetActive(!mine);
            if (storeTabRoot != null) storeTabRoot.SetActive(mine);
            Refresh();
        }

        private void Refresh()
        {
            if (_showMine)
            {
                SetStatus("Loading your store...");
                GameApi.Market.GetMine(RenderStore, OnError);
            }
            else
            {
                SetStatus("Loading market...");
                GameApi.Market.GetActive(RenderMarket, OnError);
            }
        }

        // ---------- Market tab (browse all) ----------

        private void RenderMarket(List<MarketListingDto> listings)
        {
            if (marketGridContent == null) return;

            foreach (Transform child in marketGridContent)
                Destroy(child.gameObject);

            foreach (var listing in listings)
            {
                var go = new GameObject("MarketSlot", typeof(RectTransform));
                go.transform.SetParent(marketGridContent, false);
                var slot = go.AddComponent<MarketSlotUI>();
                slot.Init();

                Sprite icon = IconFor(listing.ItemDefId);
                string seller = SellerDisplay(listing);
                int id = listing.Id;

                if (listing.Status == "Sold")
                    slot.ShowSold(icon, seller, BuyerDisplay(listing), listing.Quantity, listing.UnitPrice);
                else
                    slot.ShowForSale(icon, seller, listing.Quantity, listing.UnitPrice, () => OnBuy(id));
            }

            SetStatus(listings.Count == 0 ? "No listings on the market." : $"{listings.Count} listing(s) on the market.");
        }

        // ---------- My Store tab (slots) ----------

        private void EnsureSlots()
        {
            if (_slots.Count > 0 || storeGridContent == null) return;

            for (int i = 0; i < storeSlotCount; i++)
            {
                var go = new GameObject($"Slot{i}", typeof(RectTransform));
                go.transform.SetParent(storeGridContent, false);
                var slot = go.AddComponent<StoreSlotUI>();
                slot.Init();
                _slots.Add(slot);
            }
        }

        private void RenderStore(List<MarketListingDto> listings)
        {
            EnsureSlots();
            if (_slots.Count == 0) return;

            int slotIndex = 0;
            foreach (var listing in listings)
            {
                if (slotIndex >= _slots.Count) break;

                var slot = _slots[slotIndex++];
                Sprite icon = IconFor(listing.ItemDefId);
                int id = listing.Id;

                if (listing.Status == "Sold")
                    slot.ShowSold(icon, listing.Quantity, listing.UnitPrice, () => OnCollect(id));
                else
                    slot.ShowActive(icon, listing.Quantity, listing.UnitPrice, () => OnCancel(id));
            }

            for (int i = slotIndex; i < _slots.Count; i++)
                _slots[i].ShowEmpty(OpenPicker);

            SetStatus($"Your store: {slotIndex}/{_slots.Count} slot(s) in use.");
        }

        // ---------- Sell picker ----------

        private void OpenPicker()
        {
            if (pickerRoot == null) return;

            _pickerSelected = null;
            if (pickerSelectedLabel != null) pickerSelectedLabel.text = "Pick an item to sell";
            if (pickerQtyInput != null) pickerQtyInput.text = "";
            if (pickerPriceInput != null) pickerPriceInput.text = "";

            PopulatePickerList();
            pickerRoot.SetActive(true);
        }

        private void ClosePicker()
        {
            if (pickerRoot != null) pickerRoot.SetActive(false);
        }

        private void PopulatePickerList()
        {
            if (pickerListContent == null) return;

            foreach (Transform child in pickerListContent)
                Destroy(child.gameObject);

            if (InventoryManager.Instance == null || !InventoryManager.Instance.IsInitialized)
            {
                SetStatus("Inventory not ready.");
                return;
            }

            var sellable = InventoryManager.Instance.GetSellableInventory();
            if (sellable.Count == 0)
            {
                var empty = UIFactory.Text(pickerListContent, "Nothing sellable in your inventory.", 20,
                    TextAlignmentOptions.Center);
                UIFactory.SetHeight(empty.gameObject, 40);
                return;
            }

            foreach (var entry in sellable)
            {
                var item = entry.Key;
                int owned = entry.Value;
                string label = string.IsNullOrEmpty(item.Name) ? item.Id : item.Name;
                var captured = item;
                var btn = UIFactory.Button(pickerListContent, $"{label}  (x{owned})", () => SelectPickerItem(captured),
                    new Color(0.55f, 0.7f, 0.9f));
                UIFactory.SetHeight(btn.gameObject, 44);
            }
        }

        private void SelectPickerItem(BaseData item)
        {
            _pickerSelected = item;
            string label = string.IsNullOrEmpty(item.Name) ? item.Id : item.Name;
            int owned = InventoryManager.Instance != null ? InventoryManager.Instance.GetItemQuantity(item) : 0;
            if (pickerSelectedLabel != null) pickerSelectedLabel.text = $"Selling: {label}  (own {owned})";
        }

        private void OnPickerConfirm()
        {
            if (_pickerSelected == null) { SetStatus("Pick an item first."); return; }

            int owned = InventoryManager.Instance != null ? InventoryManager.Instance.GetItemQuantity(_pickerSelected) : 0;
            if (!int.TryParse(pickerQtyInput != null ? pickerQtyInput.text : "", out var qty) || qty <= 0)
            { SetStatus("Enter a valid quantity."); return; }
            if (qty > owned) { SetStatus($"You only own {owned}."); return; }
            if (!int.TryParse(pickerPriceInput != null ? pickerPriceInput.text : "", out var price) || price <= 0)
            { SetStatus("Enter a valid price."); return; }

            SetStatus("Posting listing...");
            string itemId = _pickerSelected.Id;
            GameApi.Market.CreateListing(itemId, qty, price,
                listing =>
                {
                    SetStatus($"Listed {listing.Quantity}x {DisplayName(listing.ItemDefId)}.");
                    ClosePicker();
                    SyncAndRefresh();
                },
                OnError);
        }

        // ---------- Actions ----------

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

        private void OnCollect(int listingId)
        {
            SetStatus("Collecting...");
            GameApi.Market.Collect(listingId,
                coins =>
                {
                    SetStatus($"Collected! Balance: {coins.Coins} coins.");
                    SyncAndRefresh();
                },
                OnError);
        }

        private void SyncAndRefresh()
        {
            if (ServerSyncManager.Instance != null)
                ServerSyncManager.Instance.PullFromServer();
            Refresh();
        }

        // ---------- Helpers ----------

        private static Sprite IconFor(string itemDefId)
        {
            if (InventoryManager.Instance == null) return null;
            var data = InventoryManager.Instance.GetPreviewDataById(itemDefId);
            return data != null ? data.icon : null;
        }

        private static string SellerDisplay(MarketListingDto listing)
            => !string.IsNullOrEmpty(listing.SellerName) ? listing.SellerName : $"Player #{listing.SellerId}";

        private static string BuyerDisplay(MarketListingDto listing)
            => !string.IsNullOrEmpty(listing.BuyerName)
                ? listing.BuyerName
                : (listing.BuyerId > 0 ? $"Player #{listing.BuyerId}" : "");

        private static string DisplayName(string itemDefId)
        {
            if (InventoryManager.Instance == null) return itemDefId;
            var data = InventoryManager.Instance.GetPreviewDataById(itemDefId);
            if (data == null) return itemDefId;
            return string.IsNullOrEmpty(data.Name) ? itemDefId : data.Name;
        }

        private void OnError(ApiError err) => SetStatus("Error: " + err.Message);

        private void SetStatus(string message)
        {
            if (status != null) status.text = message;
        }
    }
}
