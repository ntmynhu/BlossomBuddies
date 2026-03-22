using UnityEngine;

public class PlacementHarvestState : PlacementBaseState
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
            if (targetPlant.IsFullyGrown)
            {
                targetPlant.ClearWateredSoil();
                targetPlant.HarvestPlant();
                placementSystem.RemoveObject(gridPosition);
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

            if (targetPlant != null && (targetPlant.IsFullyGrown))
            {
                return true;
            }
        }

        return false;
    }
}
