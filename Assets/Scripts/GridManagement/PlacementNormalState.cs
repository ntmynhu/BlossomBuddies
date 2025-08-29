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
        
    }
}
