using System.Collections.Generic;
using UnityEngine;

public class PlacementExplosiveState : PlacementBaseState
{
    private List<string> allowedTargetIds = new();

    public void SetTargetIds(List<string> ids)
    {
        allowedTargetIds = ids;
    }

    public override void EnterState(PlacementSystem placementSystem)
    {
        placementSystem.CellIndicator.gameObject.SetActive(true);
        placementSystem.HideIndicatorObject(true);
    }

    public override void ExitState(PlacementSystem placementSystem)
    {
        placementSystem.CellIndicator.gameObject.SetActive(false);
        placementSystem.HideIndicatorObject(false);
    }

    public override void UpdateState(PlacementSystem placementSystem)
    {
        HandleIndicator(placementSystem);
    }

    private void HandleIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.MainGrid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.MainGrid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.gameObject.SetActive(CanTriggerAction(placementSystem));
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        var envGrid = placementSystem.GridDataDictionary[GridType.EnvironmentGrid];
        if (!envGrid.ContainsPosition(gridPosition)) return false;

        var placementData = envGrid.GetPlacementData(gridPosition);
        return placementData != null && allowedTargetIds.Contains(placementData.placedObjectId);
    }

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        placementSystem.RemoveObjectFromGrid(gridPosition, GridType.EnvironmentGrid);
    }
}
