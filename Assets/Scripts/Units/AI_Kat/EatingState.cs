using UnityEngine;

public class EatingState : BaseState
{
    private float foodRecoveryRate = 5f;
    private float acceptableDistance = 0.25f;

    public override void EnterState(StateManager cat)
    {
        cat.transform.LookAt(PetManager.Instance.FoodPosition.position);
        cat.Animator.SetBool("IsEating", true);
    }

    public override void UpdateState(StateManager cat)
    {
        cat.Food += foodRecoveryRate * Time.deltaTime;

        if (cat.Food >= 100f)
        {
            cat.ChangeState(cat.walkAroundState);
        }
    }

    public override void ExitState(StateManager cat)
    {
        cat.Animator.SetBool("IsEating", false);
    }
}
