using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantProgressData
{
    public int plantDataId;
    public Vector3Int mainPosition;
    public int currentStateIndex;
    public float currentGrowthTime;
}

[System.Serializable]
public class GameData
{
    public long lastLoginTime;
    public List<GridData> gridDataList;
    public GridData dualGridData;
    public List<PlantProgressData> plantProgressDataList;

    public GameData()
    {
        gridDataList = new List<GridData>();
        dualGridData = new GridData(GridType.SoilGrid);
        plantProgressDataList = new List<PlantProgressData>();
    }
}
