using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementScissorsState : PlacementBaseState
{
    private Plant targetPlant;

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
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.MainGrid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.MainGrid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;

        placementSystem.CellIndicator.gameObject.SetActive(CanTriggerAction(placementSystem));
    }

    public override void TriggerAction(PlacementSystem placementSystem)
    {
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
            else if (targetPlant.IsFullyGrown)
            {
                targetPlant.HarvestPlant();
            }
        }
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        // If there is a plant at the grid position and it belongs to the current grid data (plantgrid)
        if (placementSystem.CurrentSelectedGridData.ContainsPosition(gridPosition))
        {
            // Get target plant
            GameObject targetObject = placementSystem.GetMainGridPlacedObject(GridType.PlantGrid, gridPosition);
            targetPlant = targetObject != null ? targetObject.GetComponent<Plant>() : null;

            if (targetPlant != null && (targetPlant.IsFullyGrown || targetPlant.IsWeeded || targetPlant.IsDead))
            {
                return true;
            }
        }

        return false;
    }
}
