using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData
{
    [SerializeField] private GridType gridType;
    [SerializeField] private List<PlacementData> placedObjects;

    public GridData(GridType gridType, List<PlacementData> placementDatas = null)
    {
        this.gridType = gridType;
        placedObjects = placementDatas ?? new List<PlacementData>();
    }

    public void AddObject(Vector3Int gridPosition, Vector2Int objectSize, int Id, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new(gridPosition, positionToOccupy, Id, placedObjectIndex);

        foreach (var position in positionToOccupy)
        {
            if (ContainsPosition(position))
            {
                throw new System.Exception($"Dictionary already contains this position!");
            }
        }

        placedObjects.Add(data);
    }

    public void RemoveObject(Vector3Int gridPosition)
    {
        PlacementData dataToRemove = null;

        foreach (var placement in placedObjects)
        {
            if (placement.mainPosition == gridPosition || placement.occupiedPositions.Contains(gridPosition))
            {
                dataToRemove = placement;
                break;
            }
        }

        if (dataToRemove != null)
        {
            placedObjects.Remove(dataToRemove);
        }
    }

    public List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new List<Vector3Int>();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        var positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var position in positionToOccupy)
        {
            if (ContainsPosition(position))
            {
                return false;
            }
        }

        return true;
    }

    public bool ContainsPosition(Vector3Int position)
    {
        foreach (var placement in placedObjects)
        {
            if (placement.mainPosition == position || placement.occupiedPositions.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    public GridType GetGridType()
    {
        return gridType;
    }

    public List<PlacementData> GetPlacedObjects()
    {
        return placedObjects;
    }
}

[System.Serializable]
public class PlacementData
{
    public Vector3Int mainPosition;
    public List<Vector3Int> occupiedPositions;
    public int placedObjectId;
    public int placedObjectIndex;

    public PlacementData(Vector3Int mainPosition, List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.mainPosition = mainPosition;
        this.occupiedPositions = occupiedPositions;
        placedObjectId = iD;
        this.placedObjectIndex = placedObjectIndex;
    }
}

[System.Serializable]
public enum GridType
{
    SoilGrid,
    PlantGrid,
    FloorGrid,
    Furniture,
}
