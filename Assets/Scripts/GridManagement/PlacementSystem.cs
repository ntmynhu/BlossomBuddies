using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementSystem : Singleton<PlacementSystem>, IDataPersistence
{
    #region Fields
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private Grid grid;
    [SerializeField] private List<GridType> gridTypeList;
    [SerializeField] private ObjectsDatabaseSO databaseSO;

    private int currentSelectedIndex;
    private ObjectData currentSelectedObjectData;
    private GridData currentSelectedGridData;

    private Dictionary<GridType, List<GameObject>> placedObjects = new();

    private bool showAddIndicator = false;
    private bool showRemoveIndicator = false;

    private Dictionary<GridType, GridData> gridDataDictionary = new();

    private List<PlantProgressData> plantProgressDatas = new();

    private PlacementBaseState currentState;
    #endregion

    #region Properties
    public PlacementNormalState NormalState = new PlacementNormalState();
    public PlacementAddState AddState = new PlacementAddState();
    public PlacementRemoveState RemoveState = new PlacementRemoveState();
    public PlacementPlantState PlantState = new PlacementPlantState();

    public Dictionary<GridType, GridData> GridDataDictionary => gridDataDictionary;
    public Dictionary<GridType, List<GameObject>> PlacedObjects => placedObjects;

    public GameObject CellIndicator => cellIndicator;
    public Grid Grid => grid;

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

        if (selectedObjectData != null)
        {
            currentSelectedIndex = selectedObjectData.ID;
            currentSelectedObjectData = selectedObjectData;
            currentSelectedGridData = gridDataDictionary[selectedObjectData.gridType];
        }

        currentState = newState;
        currentState.EnterState(this);
    }

    public bool CanTriggerAction()
    {
        return currentState.CanTriggerAction(this);
    }

    public void TriggerAction()
    {
        currentState.TriggerAction(this);
    }

    public GameObject PlaceObject(Vector3Int gridPosition)
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

        return newGameObject;
    }

    public void RemoveObject(Vector3Int gridPosition)
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
    #endregion
}
