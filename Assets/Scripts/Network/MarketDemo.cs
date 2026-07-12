using System;
using UnityEngine;

namespace BlossomBuddies.Network
{
    /// <summary>
    /// End-to-end smoke test for the marketplace. Put this on the same GameObject as
    /// ApiClient (or any object) and press Play, or call RunDemo() from a button.
    /// It registers a seller + buyer, lists an item, buys it, and logs each step.
    /// Usernames are randomized so it can be run repeatedly.
    /// </summary>
    public class MarketDemo : MonoBehaviour
    {
        [SerializeField] private bool runOnStart = true;
        [SerializeField] private string itemDefId = "1003_BB";
        [SerializeField] private int quantity = 2;
        [SerializeField] private int unitPrice = 50;

        private string _sellerUser;
        private string _buyerUser;

        private void Start()
        {
            if (runOnStart) RunDemo();
        }

        [ContextMenu("Run Demo")]
        public void RunDemo()
        {
            if (ApiClient.Instance == null)
            {
                Debug.LogError("[MarketDemo] No ApiClient in scene. Add the ApiClient component first.");
                return;
            }

            var suffix = Guid.NewGuid().ToString("N").Substring(0, 6);
            _sellerUser = "seller_" + suffix;
            _buyerUser = "buyer_" + suffix;

            Debug.Log("[MarketDemo] Registering seller: " + _sellerUser);
            GameApi.Auth.Register(_sellerUser, "123", _ => OnSellerReady(), LogError("register seller"));
        }

        private void OnSellerReady()
        {
            Debug.Log("[MarketDemo] Seller logged in. Granting items...");
            GameApi.Inventory.GrantItem(itemDefId, quantity + 3,
                item => { Debug.Log($"[MarketDemo] Granted {item.Quantity}x {item.ItemDefId}"); CreateListing(); },
                LogError("grant item"));
        }

        private void CreateListing()
        {
            GameApi.Market.CreateListing(itemDefId, quantity, unitPrice,
                listing =>
                {
                    Debug.Log($"[MarketDemo] Listing #{listing.Id} created ({listing.Quantity}x @ {listing.UnitPrice}).");
                    RegisterBuyer(listing.Id);
                },
                LogError("create listing"));
        }

        private void RegisterBuyer(int listingId)
        {
            Debug.Log("[MarketDemo] Registering buyer: " + _buyerUser);
            // Registering swaps the stored token to the buyer.
            GameApi.Auth.Register(_buyerUser, "123",
                _ => GameApi.Inventory.GrantCoins(unitPrice * quantity + 100,
                    coins => { Debug.Log($"[MarketDemo] Buyer coins: {coins.Coins}"); BuyListing(listingId); },
                    LogError("grant coins")),
                LogError("register buyer"));
        }

        private void BuyListing(int listingId)
        {
            GameApi.Market.Buy(listingId,
                tx =>
                {
                    Debug.Log($"[MarketDemo] Bought listing #{tx.ListingId} for {tx.TotalPrice} coins.");
                    VerifyInventory();
                },
                LogError("buy"));
        }

        private void VerifyInventory()
        {
            GameApi.Inventory.Get(
                inv =>
                {
                    Debug.Log($"[MarketDemo] Buyer now has {inv.Coins} coins and {inv.Items.Count} item stack(s).");
                    foreach (var it in inv.Items)
                        Debug.Log($"   - {it.ItemDefId} x{it.Quantity}");
                    Debug.Log("[MarketDemo] DONE ✅");
                },
                LogError("get inventory"));
        }

        private Action<ApiError> LogError(string step)
            => err => Debug.LogError($"[MarketDemo] Failed at '{step}': {err}");
    }
}
