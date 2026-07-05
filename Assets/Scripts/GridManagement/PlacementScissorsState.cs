using System.Collections.Generic;
using UnityEngine;

public class PlacementScissorsState : PlacementBaseState
{
    private static readonly HashSet<string> GrassIds = new HashSet<string> { "100", "101", "102" };

    private Plant targetPlant;
    private bool targetIsGrass;

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

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        if (targetIsGrass)
        {
            placementSystem.RemoveObjectFromGrid(gridPosition, GridType.EnvironmentGrid);
            return;
        }

        if (targetPlant != null)
        {
            if (targetPlant.IsDead)
            {
                targetPlant.ClearWateredSoil();
                placementSystem.RemoveObject(gridPosition);
            }
            else if (targetPlant.IsWeeded)
            {
                targetPlant.CutWeed();
            }
        }
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        targetPlant = null;
        targetIsGrass = false;

        // Check for grass in EnvironmentGrid
        var envGrid = placementSystem.GridDataDictionary[GridType.EnvironmentGrid];
        if (envGrid.ContainsPosition(gridPosition))
        {
            var data = envGrid.GetPlacementData(gridPosition);
            if (data != null && GrassIds.Contains(data.placedObjectId))
            {
                targetIsGrass = true;
                return true;
            }
        }

        // Check for dead/weeded plant in PlantGrid
        if (placementSystem.CurrentSelectedGridData.ContainsPosition(gridPosition))
        {
            GameObject targetObject = placementSystem.GetMainGridPlacedObject(GridType.PlantGrid, gridPosition);
            targetPlant = targetObject != null ? targetObject.GetComponent<Plant>() : null;

            if (targetPlant != null && (targetPlant.IsWeeded || targetPlant.IsDead))
            {
                return true;
            }
        }

        return false;
    }
}
