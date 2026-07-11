using System;
using System.Collections.Generic;

namespace BlossomBuddies.Network
{
    /// <summary>
    /// Typed wrappers around the server endpoints. Each call takes success/error callbacks.
    /// Usage: GameApi.Auth.Login(u, p, onOk, onErr);
    /// </summary>
    public static class GameApi
    {
        private static ApiClient Client => ApiClient.Instance;

        public static class Auth
        {
            // On success the JWT is stored in ApiClient automatically.
            public static void Register(string username, string password,
                Action<AuthResponse> onSuccess, Action<ApiError> onError)
            {
                Client.Post<AuthResponse>("api/Authentication/register",
                    new AuthRequest { Username = username, Password = password },
                    res => { Client.SetToken(res.Token); onSuccess?.Invoke(res); },
                    onError);
            }

            public static void Login(string username, string password,
                Action<AuthResponse> onSuccess, Action<ApiError> onError)
            {
                Client.Post<AuthResponse>("api/Authentication/login",
                    new AuthRequest { Username = username, Password = password },
                    res => { Client.SetToken(res.Token); onSuccess?.Invoke(res); },
                    onError);
            }
        }

        public static class Inventory
        {
            public static void Get(Action<InventoryResponse> onSuccess, Action<ApiError> onError)
                => Client.Get("api/inventory", onSuccess, onError);

            // Dev/testing only.
            public static void GrantItem(string itemDefId, int quantity,
                Action<InventoryItemDto> onSuccess, Action<ApiError> onError)
                => Client.Post("api/inventory/grant",
                    new GrantItemRequest { ItemDefId = itemDefId, Quantity = quantity },
                    onSuccess, onError);

            // Dev/testing only.
            public static void GrantCoins(int amount,
                Action<CoinsResponse> onSuccess, Action<ApiError> onError)
                => Client.Post("api/inventory/grant-coins",
                    new GrantCoinsRequest { Amount = amount }, onSuccess, onError);

            // Pushes the local inventory + coins snapshot to the server (hybrid checkpoint).
            public static void Sync(int coins, List<InventoryItemDto> items,
                Action<string> onSuccess, Action<ApiError> onError)
                => Client.Post("api/inventory/sync",
                    new SyncInventoryRequest { Coins = coins, Items = items }, onSuccess, onError);
        }

        public static class Market
        {
            public static void GetActive(Action<List<MarketListingDto>> onSuccess, Action<ApiError> onError)
                => Client.Get("api/market", onSuccess, onError);

            public static void GetMine(Action<List<MarketListingDto>> onSuccess, Action<ApiError> onError)
                => Client.Get("api/market/my", onSuccess, onError);

            public static void CreateListing(string itemDefId, int quantity, int unitPrice,
                Action<MarketListingDto> onSuccess, Action<ApiError> onError)
                => Client.Post("api/market",
                    new CreateListingRequest { ItemDefId = itemDefId, Quantity = quantity, UnitPrice = unitPrice },
                    onSuccess, onError);

            public static void Buy(int listingId,
                Action<MarketTransactionDto> onSuccess, Action<ApiError> onError)
                => Client.Post<MarketTransactionDto>($"api/market/{listingId}/buy", null, onSuccess, onError);

            public static void Cancel(int listingId,
                Action<string> onSuccess, Action<ApiError> onError)
                => Client.Delete($"api/market/{listingId}", onSuccess, onError);
        }

        // Per-account cloud save of the whole GameData JSON blob (garden, plants, grid…).
        public static class Save
        {
            // payload.Json is null/empty for a brand-new account.
            public static void Get(Action<SavePayload> onSuccess, Action<ApiError> onError)
                => Client.Get("api/save", onSuccess, onError);

            public static void Push(string json, Action<string> onSuccess, Action<ApiError> onError)
                => Client.Post("api/save", new SavePayload { Json = json }, onSuccess, onError);
        }
    }
}
