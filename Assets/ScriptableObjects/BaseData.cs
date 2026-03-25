using UnityEngine;

[CreateAssetMenu(fileName = "BaseData", menuName = "Scriptable Objects/BaseData")]
public class BaseData : ScriptableObject
{
    public string Id;
    public string Name;
    public Sprite icon;
    public GameObject prefab;

    [Header("Shop Settings")]
    public bool isSellable;
    public int buyPrice;

    [Tooltip("If true, only one instance of this item can be owned by the player")]
    public bool isUnique;
}
