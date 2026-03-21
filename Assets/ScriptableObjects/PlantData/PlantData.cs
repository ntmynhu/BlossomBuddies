using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Scriptable Objects/PlantData")]
public class PlantData : ObjectData
{
    [Header("Plant Data")]
    public string plantName;
    public List<PlantMainStat> defaultMainStats;
    public List<PlantState> plantStates;

    [Header("Plant Appearance")]
    public List<PlantAppearance> normalAppearances;
    public List<PlantAppearance> hybridAppearances;
}

[System.Serializable]
public class PlantState
{
    [Tooltip("Time in hours (In Game) when the plant changes to the next state")]
    public float growthTime; // Hour in game
    public float deadTime;
    public List<PlantStateCondition> conditions; // Conditions for the plant to change to the next state
}

[Serializable]
public class PlantMainStat
{
    public PlantMainStatsType type;
    public float value;
}

[Serializable]
public class PlantStateCondition
{
    public PlantMainStatsType type;
    public Gradient conditionRange;
}

[Serializable]
public enum PlantMainStatsType
{
    Light,
    Water,
    Nutrient
}

[Serializable] 
public class PlantAppearance
{
    public AlenType[] alenType;
    public Sprite sprite;
    public GameObject plantPrefab;
}

[Serializable]
public enum AlenType
{
    R, Y, B, W
}