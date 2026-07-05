using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Scriptable Objects/ObjectData")]
public class ObjectData : BaseData
{
    [Header("Object Settings")]
    [Tooltip("The size of the object in grid units (x - z)")]
    public Vector2Int Size = Vector2Int.one; 
    public GridType gridType;
    public bool isInventoryItem = true;
}
