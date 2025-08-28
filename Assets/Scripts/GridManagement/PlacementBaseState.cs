using UnityEngine;

public abstract class PlacementBaseState
{
    protected Vector3 playerPosition;
    protected Vector3Int gridPosition;
    protected Vector3 targetIndicatorPosition;

    public abstract void EnterState(PlacementSystem placementSystem);
    public abstract void UpdateState(PlacementSystem placementSystem);
    public abstract void ExitState(PlacementSystem placementSystem);
    public virtual void TriggerAction(PlacementSystem placementSystem) { }
    public virtual bool CanTriggerAction(PlacementSystem placementSystem) { return false; }
}
