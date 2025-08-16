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

    private Dictionary<GridType, List<GameObject>> placedObjects = new();

    private bool showIndicator = false;

    private Vector3 playerPosition;
    private Vector3Int gridPosition;

    private Dictionary<GridType, GridData> gridDataDictionary = new();

    private List<PlantProgressData> plantProgressDatas = new();

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

            Plant plant = newGameObject.GetComponent<Plant>();
            if (plant != null)
            {
                plant.MainPosition = gridPosition;
            }

            placedObjects[objectData.gridType].Add(newGameObject);

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
