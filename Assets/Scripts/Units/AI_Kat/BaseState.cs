using UnityEngine;

public abstract class BaseState
{
    public abstract void EnterState(StateManager cat);
    public abstract void UpdateState(StateManager cat);
    public abstract void ExitState(StateManager cat);
    public abstract void OnCollisionEnter(StateManager cat, Collision collision);
    public abstract void OnTriggerEnter(StateManager cat, Collider other);
}
