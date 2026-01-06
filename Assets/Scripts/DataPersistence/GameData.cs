using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantProgressData
{
    public int plantDataId;
    public Vector3Int mainPosition;
    public int currentStateIndex;
    public float currentGrowthTime;
    public float yPosition;
    public float waterTimer;
    public int waterState;
    public bool isWatered;
    public float tickTimer;
    public List<GrassData> grassDataList;
}

[System.Serializable]
public class GrassData
{
    public bool isActive;
    public Vector3 localScale;
}

[System.Serializable]
public class GameData
{
    public int currentHeart;
    public long lastLoginTime;
    public List<GridData> gridDataList;
    public List<GridData> dualGridDataList;
    public List<PlantProgressData> plantProgressDataList;

    public GameData()
    {
        currentHeart = 0;
        gridDataList = new List<GridData>();
        dualGridDataList = new List<GridData>();
        plantProgressDataList = new List<PlantProgressData>();
    }
}
