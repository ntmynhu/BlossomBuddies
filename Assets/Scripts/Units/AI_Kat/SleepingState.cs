using UnityEngine;

public class SleepingState : BaseState
{
    private float energyRecoveryRate = 5f;
    private float happinessConsumptionRate = 1f;

    public override void EnterState(StateManager cat)
    {
        cat.Animator.SetBool("IsSleeping", true);
    }

    public override void UpdateState(StateManager cat)
    {
        cat.Energy += energyRecoveryRate * Time.deltaTime;
        cat.Happiness -= happinessConsumptionRate * Time.deltaTime;

        if (cat.Energy >= 100f)
        {
            cat.ChangeState(cat.walkAroundState);
        }
    }

    public override void OnCollisionEnter(StateManager cat, Collision collision)
    {

    }

    public override void ExitState(StateManager cat)
    {
        cat.Animator.SetBool("IsSleeping", false);
    }

    public override void OnTriggerEnter(StateManager cat, Collider other)
    {
        
    }
}
