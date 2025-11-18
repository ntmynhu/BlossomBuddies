using System.Collections.Generic;
using UnityEngine;

public class PlacementShovelState : PlacementBaseState
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
                    placementSystem.CurrentSelectedGridData.RemoveObject(gridPosition);
                    ProcessDualGridVisual(placementSystem);
                    return;
                }
            }

            Debug.Log("Other Object Existing! Replace it");
            placementSystem.RemoveObject(gridPosition);
        }

        placementSystem.AddObjectToGridData(gridPosition);
        ProcessDualGridVisual(placementSystem);
    }

    private void ProcessDualGridVisual(PlacementSystem placementSystem)
    {
        // Get 4 dural grid's positions from 1 cell in main grid
        List<Vector3Int> dualPositionsToProcess = GetPositionsToProcess(gridPosition);

        foreach (var pos in dualPositionsToProcess)
        {
            if (!placementSystem.DualGridDataDictionary[placementSystem.CurrentSelectedGridData.GridType].CanPlaceAt(pos, placementSystem.CurrentSelectedObjectData.Size))
            {
                placementSystem.RemoveObjectInDualGrid(pos, placementSystem.CurrentSelectedGridData.GridType);
            }

            Tile tile = placementSystem.PlaceAndAddObjectInDualGrid(pos, GridType.SoilGrid, placementSystem.CurrentSelectedObjectData, false).GetComponent<Tile>();
            if (tile != null)
            {
                // For each dual pos, get 4 main position to calculate tile's visual
                List<Vector3Int> mainPositionsToProcessTile = GetPositionsToProcessTile(pos);

                List<int> objectIdsToUpdateVisual = new List<int>();
                foreach (var position in mainPositionsToProcessTile)
                {
                    PlacementData placementData = placementSystem.CurrentSelectedGridData.GetPlacementData(position);
                    int objectId = (placementData != null) ? placementData.placedObjectId : -1;
                    objectIdsToUpdateVisual.Add(objectId);
                }

                Debug.Log("Updating tile visual at position: " + pos);
                tile.CalculateTileVisual(objectIdsToUpdateVisual);
            }
        }

        Debug.Log(gridPosition);
    }

    /// <summary>
    /// Get dual grid positions from main grid position
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public List<Vector3Int> GetPositionsToProcess(Vector3Int gridPosition)
    {
        List<Vector3Int> positions = new();
        positions.Add(new Vector3Int(gridPosition.x + 1, gridPosition.y + 1, gridPosition.z));

        positions.Add(new Vector3Int(gridPosition.x, gridPosition.y + 1, gridPosition.z));
        positions.Add(new Vector3Int(gridPosition.x, gridPosition.y, gridPosition.z));
        positions.Add(new Vector3Int(gridPosition.x + 1, gridPosition.y, gridPosition.z));
        return positions;
    }


    /// <summary>
    /// Get main grid positions from dual grid position
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public List<Vector3Int> GetPositionsToProcessTile(Vector3Int gridPosition)
    {
        List<Vector3Int> positions = new();
        positions.Add(new Vector3Int(gridPosition.x, gridPosition.y, gridPosition.z));

        positions.Add(new Vector3Int(gridPosition.x - 1, gridPosition.y, gridPosition.z));
        positions.Add(new Vector3Int(gridPosition.x - 1, gridPosition.y - 1, gridPosition.z));
        positions.Add(new Vector3Int(gridPosition.x, gridPosition.y - 1, gridPosition.z));
        return positions;
    }

    public override bool CanTriggerAction(PlacementSystem placementSystem)
    {
        return !placementSystem.CurrentSelectedGridData.ContainsPosition(gridPosition) ||
            (placementSystem.CurrentSelectedGridData.ContainsPosition(gridPosition) &&
            !placementSystem.GridDataDictionary[GridType.PlantGrid].ContainsPosition(gridPosition));
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
}
