using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialGameData", menuName = "Data/Initial Game Data")]
public class GameDataSO : ScriptableObject
{
    public List<GridData> gridDataList;
}
