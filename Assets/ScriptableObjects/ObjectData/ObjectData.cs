using UnityEngine;

[CreateAssetMenu]
public class ObjectData : PreviewData
{
    public int ID;
    public Vector2Int Size = Vector2Int.one;
    public GridType gridType;
    public bool isInventoryItem = true;
}
