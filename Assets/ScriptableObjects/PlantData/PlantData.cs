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
    public float time;
}
