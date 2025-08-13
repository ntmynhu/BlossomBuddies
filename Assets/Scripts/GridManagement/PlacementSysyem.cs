using System.Collections.Generic;
using UnityEngine;

public class PlacementSysyem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabaseSO databaseSO;

    private int selectedIndex;

    private GridData soilGridData, objectGridData;
    private List<GameObject> placedObjects = new();

    private void Start()
    {
        soilGridData = new GridData();
        objectGridData = new GridData();
    }

    private void Update()
    {
        //Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        //mouseIndicator.transform.position = mousePosition;

        Vector3 playerPosition = inputManager.GetPlayerSelectedMapPosition();
        Debug.Log(playerPosition);

        Vector3Int gridPosition = grid.WorldToCell(playerPosition);
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedIndex = 2;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GridData gridData = selectedIndex < 2 ? soilGridData : objectGridData;

            if (gridData.CanPlaceAt(gridPosition, databaseSO.objectDatas[selectedIndex].Size))
            {
                GameObject newGameObject = Instantiate(databaseSO.objectDatas[selectedIndex].prefab);
                newGameObject.transform.position = grid.CellToWorld(gridPosition);

                placedObjects.Add(newGameObject);

                gridData.AddObject(gridPosition, databaseSO.objectDatas[selectedIndex].Size, databaseSO.objectDatas[selectedIndex].ID, placedObjects.Count - 1);
            }
            else
            {
                Debug.Log("Cannot Place");
            }
        }
    }
}
