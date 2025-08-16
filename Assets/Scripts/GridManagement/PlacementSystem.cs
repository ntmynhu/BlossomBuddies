using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementSystem : Singleton<PlacementSystem>, IDataPersistence
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private List<GridType> gridTypeList;
    [SerializeField] private ObjectsDatabaseSO databaseSO;

    private int selectedIndex;

    private List<GameObject> placedObjects = new();

    private bool showIndicator = false;

    private Vector3 playerPosition;
    private Vector3Int gridPosition;

    private Dictionary<GridType, GridData> gridDataDictionary = new();

    private void Start()
    {
        cellIndicator.SetActive(showIndicator);
    }

    private void Update()
    {
        if (showIndicator)
        {
            HandleIndicator();
        }
    }

    private void HandleIndicator()
    {
        playerPosition = inputManager.GetPlayerSelectedMapPosition();

        gridPosition = grid.WorldToCell(playerPosition);
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }

    public void PlaceObject(int selectedIndex)
    {
        var objectData = SelectedObject(selectedIndex);
        var gridData = gridDataDictionary[objectData.gridType];

        if (gridData.CanPlaceAt(gridPosition, objectData.Size))
        {
            GameObject newGameObject = Instantiate(objectData.prefab);
            newGameObject.transform.position = grid.CellToWorld(gridPosition);

            placedObjects.Add(newGameObject);

            gridData.AddObject(gridPosition, objectData.Size, objectData.ID, placedObjects.Count - 1);
        }
        else
        {
            Debug.Log("Cannot Place");
        }
    }    

    public ObjectData SelectedObject(int ID)
    {
        var ob = databaseSO.objectDatas.Find(data => data.ID == ID);

        if (ob == null)
        {
            Debug.LogError($"Object with ID {ID} not found in database.");
            return null;
        }

        return ob;
    }    

    public void ShowIndicator(bool value)
    {
        cellIndicator.SetActive(value);
        showIndicator = value;
    }

    public void LoadData(GameData data)
    {
        foreach (var gridType in gridTypeList)
        {
            GridData storedGridData = data.gridDataList.FirstOrDefault(g => g.GetGridType() == gridType);

            if (storedGridData != null)
            {
                GridData loadedGridData = new(storedGridData.GetGridType(), storedGridData.GetPlacedObjects());

                gridDataDictionary[gridType] = loadedGridData;
                LoadExistingGrid(loadedGridData);

            }
            else
            {
                gridDataDictionary[gridType] = new GridData(gridType);
            }
        }
    }

    private void LoadExistingGrid(GridData gridData)
    {
        var placedObjects = gridData.GetPlacedObjects();

        if (placedObjects.Count == 0)
        {
            return;
        }

        foreach (var placedObject in placedObjects)
        {
            var placementData = placedObject;
            var objectData = SelectedObject(placementData.placedObjectId);

            GameObject newGameObject = Instantiate(objectData.prefab);
            newGameObject.transform.position = grid.CellToWorld(placedObject.mainPosition);

            this.placedObjects.Add(newGameObject);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.gridDataList = gridDataDictionary.Values.ToList();
    }
}
