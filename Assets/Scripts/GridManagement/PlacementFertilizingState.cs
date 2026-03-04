using System.Collections.Generic;
using UnityEngine;

public class PlacementFertilizingState : PlacementBaseState
{
    private Plant targetPlant;

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

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        if (!CanTriggerAction(placementSystem)) return;

        // Find target plant
        GameObject foundObject = placementSystem.GetMainGridPlacedObject(GridType.PlantGrid, gridPosition);

        if (foundObject != null)
        {
            targetPlant = foundObject.GetComponent<Plant>();
            if (targetPlant != null)
            {
                targetPlant.StartFertilizer();
            }
        }
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        return placementSystem.GridDataDictionary[GridType.PlantGrid].ContainsPosition(gridPosition) &&
                !placementSystem.GridDataDictionary[placementSystem.CurrentSelectedGridData.GridType].ContainsPosition(gridPosition);
    }

    protected void HandleIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.MainGrid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.MainGrid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.gameObject.SetActive(CanTriggerAction(placementSystem));
    }
}
