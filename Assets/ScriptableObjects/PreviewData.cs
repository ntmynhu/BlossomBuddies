using UnityEngine;

[CreateAssetMenu(fileName = "PreviewData", menuName = "Scriptable Objects/PreviewData")]
public class PreviewData : ScriptableObject
{
    public string Id;
    public string Name;
    public Sprite icon;
    public GameObject prefab;
}
