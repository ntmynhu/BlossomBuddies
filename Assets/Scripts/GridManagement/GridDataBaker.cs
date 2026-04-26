using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridDataBaker : MonoBehaviour
{
    [ContextMenu("Bake To ScriptableObject")]
    public void Bake()
    {
        Grid grid = GetComponent<Grid>();
        GameDataSO data = ScriptableObject.CreateInstance<GameDataSO>();

        data.gridDataList = new List<GridData>();

        var allPlacements = this.transform.GetComponentsInChildren<PlacementObject>();

        GridData gridData = new GridData(GridType.EnvironmentGrid);

        for (int i = 0; i < allPlacements.Length; i++)
        {
            var obj = allPlacements[i];

            if (obj.gameObject.activeSelf == false)
            {
                continue;
            }

            Vector3Int gridPosition = grid.WorldToCell(obj.transform.position);
            gridData.AddObject(gridPosition, obj.ObjectData.Size, obj.ObjectData.Id, i);
        }

        data.gridDataList.Add(gridData);

        AssetDatabase.CreateAsset(data, "Assets/InitialGameData.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("Bake xong!");
    }
}
