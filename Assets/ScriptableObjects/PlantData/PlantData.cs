using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Scriptable Objects/PlantData")]
public class PlantData : ObjectData
{
    [Header("Plant Data")]
    public string plantName;
    public List<PlantState> plantStates;
}

[System.Serializable]
public class PlantState
{
    [Tooltip("Time in hours (In Game) when the plant changes to the next state")]
    public float time; // Hour in game 
}
