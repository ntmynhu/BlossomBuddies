/// <summary>
/// Tracks which exclusive full-screen UI panels are open (shop, inventory, marketplace) so
/// only one can be shown at a time and world interactions (tool use, plant clicks) can be
/// blocked while any panel is up. Each panel sets its flag when it opens/closes.
/// </summary>
public enum UIPanel { Shop, Inventory, Market }

public static class GameUIState
{
    public static bool ShopOpen;
    public static bool InventoryOpen;
    public static bool MarketOpen;

    public static bool IsAnyPanelOpen => ShopOpen || InventoryOpen || MarketOpen;

    // True if a panel other than the given one is currently open.
    public static bool AnyOtherOpen(UIPanel self)
    {
        return (self != UIPanel.Shop && ShopOpen)
            || (self != UIPanel.Inventory && InventoryOpen)
            || (self != UIPanel.Market && MarketOpen);
    }
}
