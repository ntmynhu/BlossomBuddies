using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "ScriptableObjects/ObjectData")]
public class ObjectData : BaseData
{
    [Header("Object Settings")]
    public Vector2Int Size = Vector2Int.one;
    public GridType gridType;
    public bool isInventoryItem = true;
}
