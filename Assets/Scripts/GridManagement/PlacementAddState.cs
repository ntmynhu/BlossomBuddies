using UnityEngine;

public class PlacementAddState : PlacementBaseState
{
    public override void EnterState(PlacementSystem placementSystem)
    {
        placementSystem.CellIndicator.SetActive(true);
    }

    public override void ExitState(PlacementSystem placementSystem)
    {
        placementSystem.CellIndicator.SetActive(false);
    }

    public override void UpdateState(PlacementSystem placementSystem)
    {
        HandleIndicator(placementSystem);
    }

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        placementSystem.PlaceObject(gridPosition);
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        return placementSystem.CurrentSelectedGridData.CanPlaceAt(gridPosition, placementSystem.CurrentSelectedObjectData.Size);
    }

    private void HandleIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.Grid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.Grid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.SetActive(placementSystem.CurrentSelectedGridData.CanPlaceAt(gridPosition, placementSystem.CurrentSelectedObjectData.Size));
    }
}
