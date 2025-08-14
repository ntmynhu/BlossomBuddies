using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabaseSO databaseSO;

    private int selectedIndex;

    private GridData soilGridData, objectGridData;
    private List<GameObject> placedObjects = new();

    private bool showIndicator = false;

    private Vector3 playerPosition;
    private Vector3Int gridPosition;

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
        soilGridData = new GridData();
        objectGridData = new GridData();

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

    public void ShowIndicator(bool value)
    {
        cellIndicator.SetActive(value);
        showIndicator = value;
    }
}
