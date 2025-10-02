using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlacementSystem : Singleton<PlacementSystem>, IDataPersistence
{
    #region Fields
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private PreviewIndicator cellIndicator;
    [SerializeField] private Grid mainGrid;
    [SerializeField] private Grid dualGrid;
    [SerializeField] private List<GridType> mainGridTypeList;
    [SerializeField] private List<GridType> dualGridTypeList;
    [SerializeField] private ObjectsDatabaseSO databaseSO;

    private int currentSelectedIndex;
    private ObjectData currentSelectedObjectData;
    private GridData currentSelectedGridData;

    private Dictionary<GridType, GridData> gridDataDictionary = new();
    private Dictionary<GridType, List<GameObject>> mainGridPlacedObjects = new();

    private Dictionary<GridType, GridData> dualGridDataDictionary = new();
    private Dictionary<GridType, List<GameObject>> dualGridPlacedObjects = new();

    private List<PlantProgressData> plantProgressDatas = new();
    private PlacementBaseState currentState;
    #endregion

    #region Properties
    public PlacementNormalState NormalState = new PlacementNormalState();
    public PlacementAddState AddState = new PlacementAddState();
    public PlacementRemoveState RemoveState = new PlacementRemoveState();
    public PlacementPlantState PlantState = new PlacementPlantState();
    public PlacementFurnitureState FurnitureState = new PlacementFurnitureState();
    public PlacementDualGridState DualGridState = new PlacementDualGridState();
    public PlacementReplaceState ReplaceState = new PlacementReplaceState();
    public PlacementWateringState WateringState = new PlacementWateringState();
    public PlacementShovelState ShovelState = new PlacementShovelState();

    public Dictionary<GridType, GridData> GridDataDictionary => gridDataDictionary;
    public Dictionary<GridType, List<GameObject>> MainGridPlacedObjects => mainGridPlacedObjects;

    public Dictionary<GridType, GridData> DualGridDataDictionary => dualGridDataDictionary;

    public PreviewIndicator CellIndicator => cellIndicator;
    public Grid MainGrid => mainGrid;
    public Grid DualGrid => dualGrid;

    public int CurrentSelectedIndex => currentSelectedIndex;
    public ObjectData CurrentSelectedObjectData => currentSelectedObjectData;
    public GridData CurrentSelectedGridData => currentSelectedGridData;
    #endregion

    #region Methods
    private void Start()
    {
        currentState = NormalState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(PlacementBaseState newState, ObjectData selectedObjectData)
    {
        currentState.ExitState(this);

        SetCurrentObjectData(selectedObjectData);

        currentState = newState;
        currentState.EnterState(this);
    }

    public void HideIndicatorObject(bool value)
    {
        cellIndicator.HidePreviewObject(value);
    }

    public void SetCurrentObjectData(ObjectData newObject)
    {
        if (newObject == null)
        {
            return;
        }

        cellIndicator.UpdateIndicator(newObject.prefab, newObject.Size);

        currentSelectedIndex = newObject.ID;
        currentSelectedObjectData = newObject;
        currentSelectedGridData = gridDataDictionary[newObject.gridType];
    }

    public bool CanTriggerAction()
    {
        return currentState.CanTriggerAction(this);
    }

    public void TriggerAction()
    {
        currentState.TriggerAction(this);
    }

    public void AddObjectToGridData(Vector3Int gridPosition)
    {
        currentSelectedGridData.AddObject(gridPosition, currentSelectedObjectData.Size, currentSelectedObjectData.ID, mainGridPlacedObjects.Count - 1);
    }

    public void AddObjectToGridData(ObjectData objectData, GridType gridType, Vector3Int gridPosition)
    {
        GridDataDictionary[gridType].AddObject(gridPosition, objectData.Size, objectData.ID, mainGridPlacedObjects.Count - 1);
    }

    public void AddObjectToDualGrid(Vector3Int gridPosition, GridType gridType, ObjectData objectData)
    {
        dualGridDataDictionary[gridType].AddObject(gridPosition, Vector2Int.one, objectData.ID, mainGridPlacedObjects.Count - 1);
    }

    public GameObject PlaceObject(Vector3Int gridPosition, ObjectData objectData, Grid grid, bool keepIndicatorHeight = true)
    {
        Vector3 targetPosition = grid.CellToWorld(gridPosition);
        if (keepIndicatorHeight) targetPosition.y = cellIndicator.transform.position.y;

        GameObject newGameObject = Instantiate(objectData.prefab);
        newGameObject.transform.position = targetPosition;

        return newGameObject;
    }

    public GameObject PlaceAndAddObjectInDualGrid(Vector3Int gridPosition, GridType gridType, ObjectData objectData, bool keepIndicatorHeight = true)
    {
        var newGameObject = PlaceObject(gridPosition, objectData, dualGrid, keepIndicatorHeight);

        dualGridPlacedObjects[gridType].Add(newGameObject);
        AddObjectToDualGrid(gridPosition, gridType, objectData);

        return newGameObject;
    }

    public GameObject PlaceAndAddObject(Vector3Int gridPosition, bool keepIndicatorHeight = true)
    {
        var newGameObject = PlaceObject(gridPosition, currentSelectedObjectData, mainGrid, keepIndicatorHeight);

        mainGridPlacedObjects[currentSelectedObjectData.gridType].Add(newGameObject);
        AddObjectToGridData(gridPosition);

        return newGameObject;
    }

    public void RemoveObjectInDualGrid(Vector3Int gridPosition, GridType gridType)
    {
        List<GameObject> targetList = dualGridPlacedObjects[gridType];

        GameObject objectToRemove = targetList.FirstOrDefault(obj => obj.transform.position == dualGrid.CellToWorld(gridPosition));

        if (objectToRemove != null)
        {
            targetList.Remove(objectToRemove);
            Destroy(objectToRemove);

            dualGridDataDictionary[gridType].RemoveObject(gridPosition);
        }
        else
        {
            Debug.Log("No object found at the specified position to remove in dual grid.");
        }
    }

    public void RemoveObject(Vector3Int gridPosition)
    {
        List<GameObject> placedObjectsList = mainGridPlacedObjects[currentSelectedGridData.GridType];

        GameObject objectToRemove = placedObjectsList.FirstOrDefault(obj => mainGrid.WorldToCell(obj.transform.position) == gridPosition);

        Debug.Log(currentSelectedGridData.GridType);
        Debug.Log(mainGrid.CellToWorld(gridPosition));

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
    #endregion

    #region Save Load system
    public void LoadData(GameData data)
    {
        plantProgressDatas = data.plantProgressDataList;

        foreach (var gridType in mainGridTypeList)
        {
            GridData storedGridData = data.gridDataList.FirstOrDefault(g => g.GridType == gridType);

            if (storedGridData != null)
            {
                GridData loadedGridData = new(storedGridData.GridType, storedGridData.PlacedObjects);

                gridDataDictionary[gridType] = loadedGridData;

                if (!(data.dualGridDataList != null && data.dualGridDataList.FirstOrDefault(g => g.GridType == gridType) != null))
                {
                    LoadExistingGrid(loadedGridData);
                }
            }
            else
            {
                gridDataDictionary[gridType] = new GridData(gridType);
                mainGridPlacedObjects[gridType] = new List<GameObject>();
            }
        }

        // Initialize dual grid data
        foreach (var gridData in data.dualGridDataList)
        {
            if (gridData != null)
            {
                Debug.Log("Loading dual grid data");

                GridData storedDualGrid = gridData;
                dualGridDataDictionary[storedDualGrid.GridType] = new(storedDualGrid.GridType, storedDualGrid.PlacedObjects);

                LoadExistingDualGrid(dualGridDataDictionary[storedDualGrid.GridType]);
            }
        }

        foreach (var gridType in dualGridTypeList)
        {
            if (!dualGridDataDictionary.ContainsKey(gridType))
            {
                dualGridDataDictionary[gridType] = new GridData(gridType);
                dualGridPlacedObjects[gridType] = new List<GameObject>();
            }
        }
    }

    private void LoadExistingGrid(GridData gridData)
    {
        mainGridPlacedObjects[gridData.GridType] = new List<GameObject>();
        var gridPlacedObjects = gridData.PlacedObjects;

        if (gridPlacedObjects.Count == 0)
        {
            return;
        }

        bool isPlantGrid = gridData.GridType == GridType.PlantGrid;

        foreach (var placedObject in gridPlacedObjects)
        {
            var objectData = SelectedObject(placedObject.placedObjectId);

            GameObject newGameObject = Instantiate(objectData.prefab);
            newGameObject.transform.position = mainGrid.CellToWorld(placedObject.mainPosition);

            if (isPlantGrid)
            {
                Plant plant = newGameObject.GetComponent<Plant>();

                if (plant != null)
                {
                    PlantProgressData progressData = plantProgressDatas.FirstOrDefault(p => p.plantDataId == objectData.ID && p.mainPosition == placedObject.mainPosition);
                    plant.LoadExistingData(progressData);
                }
            }

            mainGridPlacedObjects[gridData.GridType].Add(newGameObject);
        }
    }

    private void LoadExistingDualGrid(GridData dualGridData)
    {
        dualGridPlacedObjects[dualGridData.GridType] = new List<GameObject>();
        var gridPlacedObjects = dualGridData.PlacedObjects;

        if (gridPlacedObjects.Count == 0)
        {
            return;
        }

        var mainGridData = gridDataDictionary[dualGridData.GridType];

        foreach (var placedObject in gridPlacedObjects)
        {
            var objectData = SelectedObject(placedObject.placedObjectId);

            GameObject newGameObject = Instantiate(objectData.prefab);
            newGameObject.transform.position = dualGrid.CellToWorld(placedObject.mainPosition);

            Tile tile = newGameObject.GetComponent<Tile>();
            if (tile != null)
            {
                // For each dual pos, get 4 main position to calculate tile's visual
                List<Vector3Int> mainPositionsToProcessTile = DualGridState.GetPositionsToProcessTile(placedObject.mainPosition);

                List<int> objectIdsToUpdateVisual = new List<int>();
                foreach (var position in mainPositionsToProcessTile)
                {
                    PlacementData placementData = mainGridData.GetPlacementData(position);
                    int objectId = (placementData != null) ? placementData.placedObjectId : -1;
                    objectIdsToUpdateVisual.Add(objectId);
                }

                tile.CalculateTileVisual(objectIdsToUpdateVisual);
            }

            dualGridPlacedObjects[dualGridData.GridType].Add(newGameObject);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.gridDataList = gridDataDictionary.Values.ToList();
        data.dualGridDataList = dualGridDataDictionary.Values.ToList();

        #region Plant Progress Data
        data.plantProgressDataList = new List<PlantProgressData>();

        List<GameObject> placedObjects_plants = mainGridPlacedObjects[GridType.PlantGrid];
        foreach (var placedObject in placedObjects_plants)
        {
            Plant plant = placedObject.GetComponent<Plant>();

            if (plant != null)
            {
                PlantProgressData plantData = plant.SavePlantData();
                data.plantProgressDataList.Add(plantData);
            }
        }
        #endregion
    }
    #endregion
}
