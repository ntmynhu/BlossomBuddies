using System.Collections.Generic;
using UnityEngine;

public class PlacementRemoveState : PlacementBaseState
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
        HandleRemoveIndicator(placementSystem);
    }

    private void HandleRemoveIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.Grid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.Grid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.SetActive(placementSystem.CurrentSelectedGridData.ContainsPosition(gridPosition));
    }

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        placementSystem.RemoveObject(gridPosition);
    }
}
