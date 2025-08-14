using UnityEngine;

[CreateAssetMenu]
public class ObjectData : ScriptableObject
{
    public string Name;
    public int ID;
    public Vector2Int Size = Vector2Int.one;
    public GameObject prefab;
}
