using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<GridData> gridDataList;

    public GameData()
    {
        gridDataList = new List<GridData>();
    }
}
