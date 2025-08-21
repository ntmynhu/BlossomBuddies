using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolInfo", menuName = "Scriptable Objects/ToolInfo")]
public class ToolInfo : ScriptableObject
{
    public string toolName;
    public Sprite toolIcon;
    public GameObject toolPrefab;
}
