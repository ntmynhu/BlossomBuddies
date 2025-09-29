using System.Collections.Generic;
using UnityEngine;

public class PlacementWateringState : PlacementBaseState
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
        List<GameObject> plantList = placementSystem.MainGridPlacedObjects[GridType.PlantGrid];
        GameObject foundObject = plantList.Find(obj => obj != null && placementSystem.MainGrid.WorldToCell(obj.transform.position) == gridPosition);

        if (foundObject != null)
        {
            targetPlant = foundObject.GetComponent<Plant>();
            if (targetPlant != null)
            {
                Debug.Log("Watering plant at position: " + gridPosition);
                targetPlant.StartWater(placementSystem.CurrentSelectedObjectData);
            }
        }

        placementSystem.AddObjectToGridData(gridPosition);
        ProcessDualGridVisual(placementSystem, gridPosition);
    }

    public void ProcessDualGridVisual(PlacementSystem placementSystem, Vector3Int gridPosition)
    {
        // Get 4 dural grid's positions from 1 cell in main grid
        List<Vector3Int> dualPositionsToProcess = GetPositionsToProcess(gridPosition);

        foreach (var pos in dualPositionsToProcess)
        {
            if (!placementSystem.DualGridDataDictionary[GridType.WateringGrid].CanPlaceAt(pos, targetPlant.WateredSoilData.Size))
            {
                placementSystem.RemoveObjectInDualGrid(pos, GridType.WateringGrid);
            }

            Tile tile = placementSystem.PlaceAndAddObjectInDualGrid(pos, GridType.WateringGrid, targetPlant.WateredSoilData, false).GetComponent<Tile>();
            if (tile != null)
            {
                // For each dual pos, get 4 main position to calculate tile's visual
                List<Vector3Int> mainPositionsToProcessTile = GetPositionsToProcessTile(pos);

                List<int> objectIdsToUpdateVisual = new List<int>();
                foreach (var position in mainPositionsToProcessTile)
                {
                    PlacementData placementData = placementSystem.GridDataDictionary[GridType.WateringGrid].GetPlacementData(position);
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
