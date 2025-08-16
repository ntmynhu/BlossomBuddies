using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData
{
    [SerializeField] private GridType gridType;
    [SerializeField] private List<PlacementData> placedObjects;

    public GridData(GridType gridType)
    {
        this.gridType = gridType;
        placedObjects = new List<PlacementData>();
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

    private bool ContainsPosition(Vector3Int position)
    {
        foreach (var placement in placedObjects)
        {
            if (placement.mainPosition == position)
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
    public int ID {  get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(Vector3Int mainPosition, List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.mainPosition = mainPosition;
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}

[System.Serializable]
public enum GridType
{
    SoilGrid,
    PlantGrid
}
