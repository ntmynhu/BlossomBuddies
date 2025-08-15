using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
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

    #region Singleton
    private static PlacementSystem instance;
    public static PlacementSystem Instance => instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        for (int i = 0; i < gridTypeList.Count; i++)
        {
            GridData gridData = new GridData(gridTypeList[i]);
            gridDataDictionary[gridTypeList[i]] = gridData;
        }

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
        return databaseSO.objectDatas.Find(data => data.ID == ID);
    }    

    public void ShowIndicator(bool value)
    {
        cellIndicator.SetActive(value);
        showIndicator = value;
    }
}
