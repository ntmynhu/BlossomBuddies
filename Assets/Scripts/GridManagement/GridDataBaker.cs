using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridDataBaker : MonoBehaviour
{
    [SerializeField] private GridType gridType;

    [ContextMenu("Bake To ScriptableObject")]
    public void Bake()
    {
        Grid grid = GetComponent<Grid>();
        GridDataSO data = ScriptableObject.CreateInstance<GridDataSO>();

        var allPlacements = this.transform.GetComponentsInChildren<PlacementObject>();

        GridData gridData = new GridData(gridType);

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

        data.gridData = gridData;

        AssetDatabase.CreateAsset(data, $"Assets/{gridType}Data.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("Bake xong!");
    }
}
