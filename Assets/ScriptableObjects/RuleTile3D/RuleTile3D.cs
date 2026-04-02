using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New RuleTile3D", menuName = "ScriptableObjects/RuleTile3D")]
public class RuleTile3D : ScriptableObject
{
    public TileRule[] rules;
}


[Serializable]
public class TileRule
{
    public NeighborCondition[] neighbors = new NeighborCondition[8];
    public GameObject prefab;
    public GameObject edgePrefab;
}

[Serializable]
public enum NeighborCondition
{
    Any,       // grey
    Same,      // green
    Different  // red
}