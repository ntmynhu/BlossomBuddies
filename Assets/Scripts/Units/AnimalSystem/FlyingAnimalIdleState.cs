using UnityEngine;

public class FlyingAnimalIdleState : FlyingAnimalBaseState
{
    private float duration;

    public override void EnterState(FlyingAnimalHandler handler)
    {
        handler.Animator.Play("Idle");
        duration = Random.Range(handler.IdleDurationMin, handler.IdleDurationMax);
    }

    public override void UpdateState(FlyingAnimalHandler handler)
    {
        base.UpdateState(handler);

        if (handler.IsPlayerInRange)
        {
            handler.ChangeState(handler.flyingAroundState);
        }

        duration -= Time.deltaTime;
        if (duration <= 0f)
        {
            handler.ChangeState(handler.flyingAroundState);
        }
    }

    public override void ExitState(FlyingAnimalHandler handler)
    {
        
    }
}
