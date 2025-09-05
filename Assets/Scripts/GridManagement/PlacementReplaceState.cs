using UnityEngine;

public class PlacementReplaceState : PlacementBaseState
{
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
        if (!placementSystem.CurrentSelectedGridData.CanPlaceAt(gridPosition, placementSystem.CurrentSelectedObjectData.Size))
        {
            var placementData = placementSystem.CurrentSelectedGridData.GetPlacementData(gridPosition);
            if (placementData != null)
            {
                if (placementData.placedObjectId == placementSystem.CurrentSelectedObjectData.ID)
                {
                    Debug.Log("Same Object Existing! Do Nothing");
                    return;
                }
            }

            Debug.Log("Other Object Existing! Replace it");
            placementSystem.RemoveObject(gridPosition);
        }

        placementSystem.PlaceAndAddObject(gridPosition, false);
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        return true;
    }

    private void HandleIndicator(PlacementSystem placementSystem)
    {
        playerPosition = InputManager.Instance.GetPlayerSelectedMapPosition();
        gridPosition = placementSystem.MainGrid.WorldToCell(playerPosition);
        targetIndicatorPosition = placementSystem.MainGrid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        placementSystem.CellIndicator.transform.position = targetIndicatorPosition;
        placementSystem.CellIndicator.SetValid(CanTriggerAction(placementSystem));
    }
}

