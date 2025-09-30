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
}

[System.Serializable]
public class GameData
{
    public long lastLoginTime;
    public List<GridData> gridDataList;
    public List<GridData> dualGridDataList;
    public List<PlantProgressData> plantProgressDataList;

    public GameData()
    {
        gridDataList = new List<GridData>();
        dualGridDataList = new List<GridData>();
        plantProgressDataList = new List<PlantProgressData>();
    }
}
