using UnityEngine;

public class PlacementSysyem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabaseSO databaseSO;

    private int selectedIndex;

    private void Update()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        mouseIndicator.transform.position = mousePosition;

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
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
            GameObject newGameObject = Instantiate(databaseSO.objectDatas[selectedIndex].prefab);
            newGameObject.transform.position = grid.CellToWorld(gridPosition);
        }
    }
}
