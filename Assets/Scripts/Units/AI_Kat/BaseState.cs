using UnityEngine;

public abstract class BaseState
{
    public abstract void EnterState(StateManager cat);
    public abstract void UpdateState(StateManager cat);
    public abstract void ExitState(StateManager cat);
    public virtual void OnCollisionEnter(StateManager cat, Collision collision) { }
    public virtual void OnTriggerEnter(StateManager cat, Collider other) { }
}
