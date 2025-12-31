using UnityEngine;

public class FlyingAnimalIdleState : FlyingAnimalBaseState
{
    public override void EnterState(FlyingAnimalHandler handler)
    {
        handler.Animator.Play("Idle");
        handler.transform.rotation = Quaternion.Euler(0f, handler.transform.rotation.eulerAngles.y, 0f);
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
