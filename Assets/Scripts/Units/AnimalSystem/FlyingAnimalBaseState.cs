using UnityEngine;

public abstract class FlyingAnimalBaseState
{
    public abstract void EnterState(FlyingAnimalHandler handler);
    public virtual void UpdateState(FlyingAnimalHandler handler) { }
    public abstract void ExitState(FlyingAnimalHandler handler);
    public virtual void OnCollisionEnter(FlyingAnimalHandler handler, Collision collision) { }
    public virtual void OnTriggerEnter(FlyingAnimalHandler handler, Collider other) { }
    public virtual void OnTriggerStay(FlyingAnimalHandler handler, Collider other) { }
    public virtual void OnInteract(FlyingAnimalHandler handler) { }
}
