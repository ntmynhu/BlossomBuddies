using UnityEngine;

public class PlacementPlantState : PlacementBaseState
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

    private void HandleIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.Grid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.Grid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.SetActive(CanPlantAt(placementSystem));
    }

    public override void TriggerAction(PlacementSystem placementSystem)
    {
        GameObject newGameObject = PlacementSystem.Instance.PlaceObject(gridPosition);
        Plant plant = newGameObject.GetComponent<Plant>();
        if (plant != null)
        {
            plant.MainPosition = gridPosition;
        }
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        return CanPlantAt(placementSystem);
    }

    public bool CanPlantAt(PlacementSystem placementSystem)
    {
        GridData soildGrid = placementSystem.GridDataDictionary[GridType.SoilGrid];

        // Can plant if the selected grid allows placement and the soil grid does have an object there
        bool canPlant = placementSystem.CurrentSelectedGridData.CanPlaceAt(gridPosition, placementSystem.CurrentSelectedObjectData.Size)
                        && !soildGrid.CanPlaceAt(gridPosition, placementSystem.CurrentSelectedObjectData.Size);

        return canPlant;
    }
}
