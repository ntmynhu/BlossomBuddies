using UnityEngine;

public class PlacementNormalState : PlacementBaseState
{
    public override void EnterState(PlacementSystem placementSystem)
    {
        placementSystem.CellIndicator.gameObject.SetActive(false);
    }

    public override void ExitState(PlacementSystem placementSystem)
    {
        
    }

    public override void UpdateState(PlacementSystem placementSystem)
    {
        // With no tool selected, clicking a growing plant toggles its condition indicators.
        if (!Input.GetMouseButtonDown(0)) return;

        var es = UnityEngine.EventSystems.EventSystem.current;
        if (es != null && es.IsPointerOverGameObject()) return; // ignore clicks on UI

        Vector3 worldPos = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPos = placementSystem.MainGrid.WorldToCell(worldPos);

        GameObject obj = placementSystem.GetMainGridPlacedObject(GridType.PlantGrid, gridPos);
        if (obj == null) return;

        Plant plant = obj.GetComponent<Plant>();
        if (plant != null)
            plant.ToggleConditionUI();
    }
}
