using UnityEngine;

public class EatingState : PetBaseState
{
    public override void EnterState(PetStateHandler cat)
    {
        cat.transform.LookAt(PetManager.Instance.FoodPosition.position);
        cat.Animator.SetBool("IsEating", true);

        StatsRate = cat.PetRateDict[PetStateType.Eat];
    }

    public override void UpdateState(PetStateHandler cat)
    {
        base.UpdateState(cat);

        if (cat.Food >= 100f)
        {
            cat.ChangeState(cat.walkAroundState);
        }
    }

    public override void ExitState(PetStateHandler cat)
    {
        cat.Animator.SetBool("IsEating", false);
    }
}
