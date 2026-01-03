using UnityEngine;

public class FlyingAnimalIdleState : FlyingAnimalBaseState
{
    public override void EnterState(FlyingAnimalHandler handler)
    {
        handler.Animator.Play("Idle");
    }

    public override void UpdateState(FlyingAnimalHandler handler)
    {
        base.UpdateState(handler);

        if (handler.IsPlayerInRange)
        {
            handler.ChangeState(handler.flyingAroundState);
        }
    }

    public override void ExitState(FlyingAnimalHandler handler)
    {
        
    }
}
