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

    private Dictionary<GridType, List<GameObject>> placedObjects = new();
    private Dictionary<GridType, GridData> gridDataDictionary = new();

    private GridData dualGridData;
    private List<GameObject> dualGridPlacedObjects = new();

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

    public Dictionary<GridType, GridData> GridDataDictionary => gridDataDictionary;
    public Dictionary<GridType, List<GameObject>> PlacedObjects => placedObjects;

    public GridData DualGridData => dualGridData;

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
        currentSelectedGridData.AddObject(gridPosition, currentSelectedObjectData.Size, currentSelectedObjectData.ID, placedObjects.Count - 1);
    }

    public void AddObjectToDualGrid(Vector3Int gridPosition)
    {
        dualGridData.AddObject(gridPosition, Vector2Int.one, currentSelectedObjectData.ID, placedObjects.Count - 1);
    }

    public GameObject PlaceObject(Vector3Int gridPosition, Grid grid, bool keepIndicatorHeight = true)
    {
        Vector3 targetPosition = grid.CellToWorld(gridPosition);
        if (keepIndicatorHeight) targetPosition.y = cellIndicator.transform.position.y;

        GameObject newGameObject = Instantiate(currentSelectedObjectData.prefab);
        newGameObject.transform.position = targetPosition;

        return newGameObject;
    }

    public GameObject PlaceAndAddObjectInDualGrid(Vector3Int gridPosition)
    {
        var newGameObject = PlaceObject(gridPosition, dualGrid, false);

        dualGridPlacedObjects.Add(newGameObject);
        AddObjectToDualGrid(gridPosition);

        return newGameObject;
    }

    public GameObject PlaceAndAddObject(Vector3Int gridPosition, bool keepIndicatorHeight = true)
    {
        var newGameObject = PlaceObject(gridPosition, mainGrid, keepIndicatorHeight);

        placedObjects[currentSelectedObjectData.gridType].Add(newGameObject);
        AddObjectToGridData(gridPosition);

        return newGameObject;
    }

    public void RemoveObjectInDualGrid(Vector3Int gridPosition)
    {
        GameObject objectToRemove = dualGridPlacedObjects.FirstOrDefault(obj => obj.transform.position == dualGrid.CellToWorld(gridPosition));

        if (objectToRemove != null)
        {
            dualGridPlacedObjects.Remove(objectToRemove);
            Destroy(objectToRemove);

            dualGridData.RemoveObject(gridPosition);
        }
        else
        {
            Debug.Log("No object found at the specified position to remove in dual grid.");
        }
    }

    public void RemoveObject(Vector3Int gridPosition)
    {
        List<GameObject> placedObjectsList = placedObjects[currentSelectedGridData.GetGridType()];

        GameObject objectToRemove = placedObjectsList.FirstOrDefault(obj => obj.transform.position == mainGrid.CellToWorld(gridPosition));

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
            GridData storedGridData = data.gridDataList.FirstOrDefault(g => g.GetGridType() == gridType);

            if (storedGridData != null)
            {
                GridData loadedGridData = new(storedGridData.GetGridType(), storedGridData.GetPlacedObjects());

                gridDataDictionary[gridType] = loadedGridData;

                if (!(data.dualGridData != null && gridType == data.dualGridData.GetGridType()))
                {
                    LoadExistingGrid(loadedGridData);
                }
            }
            else
            {
                gridDataDictionary[gridType] = new GridData(gridType);
                placedObjects[gridType] = new List<GameObject>();
            }
        }

        // Initialize dual grid data
        if (data.dualGridData != null)
        {
            Debug.Log("Loading dual grid data");

            GridData storedDualGrid = data.dualGridData;
            dualGridData = new(storedDualGrid.GetGridType(), storedDualGrid.GetPlacedObjects());

            LoadExistingDualGrid(dualGridData);
        }
        else
        {
            dualGridData = new GridData(GridType.SoilGrid);
            dualGridPlacedObjects = new List<GameObject>();
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

            placedObjects[gridData.GetGridType()].Add(newGameObject);
        }
    }

    private void LoadExistingDualGrid(GridData dualGridData)
    {
        dualGridPlacedObjects = new List<GameObject>();
        var gridPlacedObjects = dualGridData.GetPlacedObjects();

        if (gridPlacedObjects.Count == 0)
        {
            return;
        }

        var mainGridData = gridDataDictionary[dualGridData.GetGridType()];

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

            dualGridPlacedObjects.Add(newGameObject);
        }

        /*GridData mainGridData = gridDataDictionary[dualGridData.GetGridType()];

        foreach (var obj in mainGridData.GetPlacedObjects())
        {
            // Get 4 dural grid's positions from 1 cell in main grid
            List<Vector3Int> dualPositionsToProcess = DualGridState.GetPositionsToProcess(obj.mainPosition);

            foreach (var pos in dualPositionsToProcess)
            {
                ObjectData objectData = SelectedObject(obj.placedObjectId);

                if (!DualGridData.CanPlaceAt(pos, objectData.Size))
                {
                    RemoveObjectInDualGrid(pos);
                }

                GameObject newGameObject = Instantiate(objectData.prefab);
                newGameObject.transform.position = dualGrid.CellToWorld(pos);

                Tile tile = newGameObject.GetComponent<Tile>();

                if (tile != null)
                {
                    // For each dual pos, get 4 main position to calculate tile's visual
                    List<Vector3Int> mainPositionsToProcessTile = DualGridState.GetPositionsToProcessTile(pos);

                    List<int> objectIdsToUpdateVisual = new List<int>();
                    foreach (var position in mainPositionsToProcessTile)
                    {
                        PlacementData placementData = mainGridData.GetPlacementData(position);
                        int objectId = (placementData != null) ? placementData.placedObjectId : -1;
                        objectIdsToUpdateVisual.Add(objectId);
                    }

                    Debug.Log("Updating tile visual at position: " + pos);
                    tile.CalculateTileVisual(objectIdsToUpdateVisual);
                }
            }
        }*/
    }

    public void SaveData(ref GameData data)
    {
        data.gridDataList = gridDataDictionary.Values.ToList();
        data.dualGridData = dualGridData;

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
    #endregion
}
