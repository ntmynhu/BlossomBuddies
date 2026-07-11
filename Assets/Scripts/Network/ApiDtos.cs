using System;
using System.Collections.Generic;

namespace BlossomBuddies.Network
{
    // Field names mirror the server responses (PascalCase). Newtonsoft matches
    // case-insensitively, so these also bind fine if the server switches to camelCase.

    // ----- Auth -----
    [Serializable]
    public class AuthRequest
    {
        public string Username;
        public string Password;
    }

    [Serializable]
    public class AuthResponse
    {
        public string Token;
    }

    // ----- Inventory -----
    [Serializable]
    public class InventoryResponse
    {
        public int Coins;
        public List<InventoryItemDto> Items = new();
    }

    [Serializable]
    public class InventoryItemDto
    {
        public string ItemDefId;
        public int Quantity;
    }

    [Serializable]
    public class GrantItemRequest
    {
        public string ItemDefId;
        public int Quantity;
    }

    [Serializable]
    public class GrantCoinsRequest
    {
        public int Amount;
    }

    [Serializable]
    public class CoinsResponse
    {
        public int Coins;
    }

    [Serializable]
    public class SyncInventoryRequest
    {
        public int Coins;
        public List<InventoryItemDto> Items = new();
    }

    // ----- Cloud save (whole GameData JSON blob) -----
    [Serializable]
    public class SavePayload
    {
        public string Json;
    }

    // ----- Market -----
    [Serializable]
    public class CreateListingRequest
    {
        public string ItemDefId;
        public int Quantity;
        public int UnitPrice;
    }

    [Serializable]
    public class MarketListingDto
    {
        public int Id;
        public int SellerId;
        public string ItemDefId;
        public int Quantity;
        public int UnitPrice;
        public string Status;
        public DateTime CreatedAt;
    }

    [Serializable]
    public class MarketTransactionDto
    {
        public int Id;
        public int ListingId;
        public int BuyerId;
        public int SellerId;
        public string ItemDefId;
        public int Quantity;
        public int TotalPrice;
        public DateTime CreatedAt;
    }
}
