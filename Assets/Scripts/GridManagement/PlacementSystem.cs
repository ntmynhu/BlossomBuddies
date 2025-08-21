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

    private int currentSelectedIndex;
    private ObjectData currentSelectedObjectData;
    private GridData currentSelectedGridData;

    private Dictionary<GridType, List<GameObject>> placedObjects = new();

    private bool showAddIndicator = false;
    private bool showRemoveIndicator = false;

    private Vector3 playerPosition;
    private Vector3Int gridPosition;
    private Vector3 targetIndicatorPosition;

    private Dictionary<GridType, GridData> gridDataDictionary = new();

    private List<PlantProgressData> plantProgressDatas = new();

    private void Start()
    {
        cellIndicator.SetActive(showAddIndicator);
    }

    private void Update()
    {
        if (showAddIndicator)
        {
            HandleAddIndicator();
        }
        else if (showRemoveIndicator)
        {
            HandleRemoveIndicator();
        }
    }

    private void HandleRemoveIndicator()
    {
        playerPosition = inputManager.GetPlayerSelectedMapPosition();
        gridPosition = grid.WorldToCell(playerPosition);
        targetIndicatorPosition = grid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        cellIndicator.transform.position = targetIndicatorPosition;

        cellIndicator.SetActive(currentSelectedGridData.ContainsPosition(gridPosition));
    }

    private void HandleAddIndicator()
    {
        playerPosition = inputManager.GetPlayerSelectedMapPosition();
        gridPosition = grid.WorldToCell(playerPosition);
        targetIndicatorPosition = grid.CellToWorld(gridPosition);

        targetIndicatorPosition.y = playerPosition.y;
        cellIndicator.transform.position = targetIndicatorPosition;

        cellIndicator.SetActive(currentSelectedGridData.CanPlaceAt(gridPosition, currentSelectedObjectData.Size));

        if (currentSelectedGridData.GetGridType() == GridType.PlantGrid)
        {
            if (!CanPlantAt())
            {
                cellIndicator.SetActive(false);
            }
        }    
    }

    public void RemoveObject()
    {
        List<GameObject> placedObjectsList = placedObjects[currentSelectedGridData.GetGridType()];

        GameObject objectToRemove = placedObjectsList.FirstOrDefault(obj => obj.transform.position == grid.CellToWorld(gridPosition));

        if (objectToRemove != null)
        {
            placedObjectsList.Remove(objectToRemove);
            Destroy(objectToRemove);

            currentSelectedGridData.RemoveObject(gridPosition);
        }
        else
        {
            Debug.Log("No object found at the specified position to remove.");
        }
    }

    public void SetCurrentSelectedIndex(int newIndex)
    {
        currentSelectedIndex = newIndex;

        currentSelectedObjectData = SelectedObject(currentSelectedIndex);
        currentSelectedGridData = gridDataDictionary[currentSelectedObjectData.gridType];
    }

    public void SetCurrentSelectedGridData(GridType gridType)
    {
        currentSelectedGridData = gridDataDictionary[gridType];
    }

    public void PlaceObject()
    {
        GameObject newGameObject = Instantiate(currentSelectedObjectData.prefab);
        newGameObject.transform.position = grid.CellToWorld(gridPosition);

        Plant plant = newGameObject.GetComponent<Plant>();
        if (plant != null)
        {
            plant.MainPosition = gridPosition;
        }

        placedObjects[currentSelectedObjectData.gridType].Add(newGameObject);

        currentSelectedGridData.AddObject(gridPosition, currentSelectedObjectData.Size, currentSelectedObjectData.ID, placedObjects.Count - 1);
    }    

    public bool CanPlaceAt()
    {
        return currentSelectedGridData.CanPlaceAt(gridPosition, currentSelectedObjectData.Size);
    }

    public void Plant() // Similiar to PlaceObject, but specifically for planting objects
    {
        PlaceObject();
    }

    public bool CanPlantAt()
    {
        GridData soildGrid = gridDataDictionary[GridType.SoilGrid];

        // Means there is soil to plant on
        return !soildGrid.CanPlaceAt(gridPosition, currentSelectedObjectData.Size);
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

    public void ShowAddIndicator(bool value)
    {
        cellIndicator.SetActive(value);
        showAddIndicator = value;

        showRemoveIndicator = false;
    }

    public void ShowRemoveIndicator(bool value)
    {
        cellIndicator.SetActive(value);
        showRemoveIndicator = value;

        showAddIndicator = false;
    }

    public void LoadData(GameData data)
    {
        plantProgressDatas = data.plantProgressDataList;

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
                placedObjects[gridType] = new List<GameObject>();
            }
        }
    }

    private void LoadExistingGrid(GridData gridData)
    {
        placedObjects[gridData.GetGridType()] = new List<GameObject>();
        var gridPlacedObjects = gridData.GetPlacedObjects();

        if (gridPlacedObjects.Count == 0)
        {
            return;
        }

        bool isPlantGrid = gridData.GetGridType() == GridType.PlantGrid;

        foreach (var placedObject in gridPlacedObjects)
        {
            var placementData = placedObject;
            var objectData = SelectedObject(placementData.placedObjectId);

            GameObject newGameObject = Instantiate(objectData.prefab);
            newGameObject.transform.position = grid.CellToWorld(placedObject.mainPosition);

            if (isPlantGrid)
            {
                Plant plant = newGameObject.GetComponent<Plant>();

                if (plant != null)
                {
                    PlantProgressData progressData = plantProgressDatas.FirstOrDefault(p => p.plantDataId == objectData.ID && p.mainPosition == placedObject.mainPosition);
                    plant.LoadExistingData(progressData);
                }
            }

            placedObjects[gridData.GetGridType()].Add(newGameObject);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.gridDataList = gridDataDictionary.Values.ToList();

        #region Plant Progress Data
        data.plantProgressDataList = new List<PlantProgressData>();

        List<GameObject> placedObjects_plants = placedObjects[GridType.PlantGrid];
        foreach (var placedObject in placedObjects_plants)
        {
            Plant plant = placedObject.GetComponent<Plant>();

            if (plant != null)
            {
                PlantProgressData plantData = new PlantProgressData
                {
                    plantDataId = plant.PlantData.ID,
                    mainPosition = plant.MainPosition,
                    currentStateIndex = plant.CurrentStateIndex,
                    currentGrowthTime = plant.CurrentGrowthTime
                };

                data.plantProgressDataList.Add(plantData);
            }
        }
        #endregion
    }
}
