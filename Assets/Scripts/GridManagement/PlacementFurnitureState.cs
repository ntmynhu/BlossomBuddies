using UnityEngine;

public class PlacementFurnitureState : PlacementBaseState
{
    public override void EnterState(PlacementSystem placementSystem)
    {
        placementSystem.CellIndicator.gameObject.SetActive(true);
    }

    public override void ExitState(PlacementSystem placementSystem)
    {
        placementSystem.CellIndicator.gameObject.SetActive(false);
    }

    public override void UpdateState(PlacementSystem placementSystem)
    {
        HandleIndicator(placementSystem);
    }

    private void HandleIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetSelectedMapPosition();
        gridPosition = placementSystem.Grid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.Grid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.SetValid(CanTriggerAction(placementSystem));
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        return placementSystem.CurrentSelectedGridData.CanPlaceAt(gridPosition, placementSystem.CurrentSelectedObjectData.Size);
    }

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        // Test
        if (!CanTriggerAction(placementSystem))
        {
            placementSystem.RemoveObject(gridPosition);
        }

        placementSystem.PlaceObject(gridPosition);
    }
}
