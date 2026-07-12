using System.Collections.Generic;
using UnityEngine;

namespace BlossomBuddies.Network
{
    /// <summary>
    /// Bridges the server's authoritative inventory + coins with the local game state.
    /// Hybrid model: pull from the server on login, run gameplay locally, push a snapshot
    /// at save checkpoints. Coins map 1:1 to GameManager.CurrentHeart (Heart == server Coins).
    /// Place on the bootstrap object alongside ApiClient / SessionManager.
    /// </summary>
    public class ServerSyncManager : MonoBehaviour
    {
        public static ServerSyncManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // Note: pull/push are driven explicitly by LoadingManager once the game scene is ready
        // (and by the market after trades), so we intentionally do NOT auto-pull on OnLoggedIn —
        // that would race with new-account seeding and run before the managers exist.

        /// <summary>Loads coins + inventory from the server into the running game.</summary>
        public void PullFromServer()
        {
            GameApi.Inventory.Get(
                inv =>
                {
                    if (GameManager.Instance != null)
                        GameManager.Instance.CurrentHeart = inv.Coins;

                    if (InventoryManager.Instance != null)
                    {
                        var items = new List<KeyValuePair<string, int>>(inv.Items.Count);
                        foreach (var it in inv.Items)
                            items.Add(new KeyValuePair<string, int>(it.ItemDefId, it.Quantity));

                        InventoryManager.Instance.SetInventoryFromServer(items);
                    }

                    Debug.Log($"[Sync] Pulled {inv.Items.Count} item stack(s), {inv.Coins} coins from server.");
                },
                err => Debug.LogError("[Sync] Pull failed: " + err));
        }

        /// <summary>Pushes the current local inventory + coins snapshot to the server.</summary>
        public void PushToServer()
        {
            if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn)
                return;
            if (InventoryManager.Instance == null || GameManager.Instance == null)
                return;

            var items = new List<InventoryItemDto>();
            foreach (var kv in InventoryManager.Instance.ExportInventory())
                items.Add(new InventoryItemDto { ItemDefId = kv.Key, Quantity = kv.Value });

            var coins = GameManager.Instance.CurrentHeart;

            GameApi.Inventory.Sync(coins, items,
                _ => Debug.Log($"[Sync] Pushed {items.Count} stack(s), {coins} coins to server."),
                err => Debug.LogError("[Sync] Push failed: " + err));
        }

        // Save checkpoints. Note: a request started on quit may not finish before the app
        // closes; call PushToServer() at explicit safe points (after harvest/shop) for reliability.
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) PushToServer();
        }

        private void OnApplicationQuit()
        {
            PushToServer();
        }
    }
}
