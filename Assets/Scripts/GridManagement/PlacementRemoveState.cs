using System.Collections.Generic;
using UnityEngine;

public class PlacementRemoveState : PlacementBaseState
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
        HandleRemoveIndicator(placementSystem);
    }

    private void HandleRemoveIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.MainGrid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.MainGrid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.gameObject.SetActive(placementSystem.CurrentSelectedGridData.ContainsPosition(gridPosition));
    }

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        placementSystem.RemoveObject(gridPosition);
    }
}
