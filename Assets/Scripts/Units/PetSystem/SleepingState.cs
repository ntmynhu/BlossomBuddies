using UnityEngine;

public class SleepingState : PetBaseState
{
    public override void EnterState(PetStateHandler cat)
    {
        cat.Animator.SetBool("IsSleeping", true);
        StatsRate = cat.PetRateDict[PetStateType.Sleep];
    }

    public override void UpdateState(PetStateHandler cat)
    {
        base.UpdateState(cat);

        if (cat.Energy >= 100f)
        {
            cat.ChangeState(cat.walkAroundState);
        }
    }

    public override void OnCollisionEnter(PetStateHandler cat, Collision collision)
    {

    }

    public override void ExitState(PetStateHandler cat)
    {
        cat.Animator.SetBool("IsSleeping", false);
    }

    public override void OnTriggerEnter(PetStateHandler cat, Collider other)
    {
        
    }
}
