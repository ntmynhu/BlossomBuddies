using UnityEngine;

public class SleepingState : PetBaseState
{
    public override void EnterState(PetStateManager cat)
    {
        cat.Animator.SetBool("IsSleeping", true);
        StatsRate = cat.PetRateDict[PetStateType.Sleep];
    }

    public override void UpdateState(PetStateManager cat)
    {
        base.UpdateState(cat);

        if (cat.Energy >= 100f)
        {
            cat.ChangeState(cat.walkAroundState);
        }
    }

    public override void OnCollisionEnter(PetStateManager cat, Collision collision)
    {

    }

    public override void ExitState(PetStateManager cat)
    {
        cat.Animator.SetBool("IsSleeping", false);
    }

    public override void OnTriggerEnter(PetStateManager cat, Collider other)
    {
        
    }
}
