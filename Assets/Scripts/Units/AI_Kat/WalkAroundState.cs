using UnityEngine;

public class WalkAroundState : BaseState
{
    private float waitingIntervalMin = 1f;
    private float waitingIntervalMax = 5f;

    private float waitingTime;

    private float movingRadius = 10f;
    private Vector3 targetPosition;

    private float energyConsumption = 10f;

    public override void EnterState(StateManager cat)
    {
        waitingTime = Random.Range(waitingIntervalMin, waitingIntervalMax);
    }

    public override void UpdateState(StateManager cat)
    {
        float currentSpeed = cat.NavMeshAgent.velocity.magnitude;

        if (currentSpeed <= 0)
        {
            waitingTime -= Time.deltaTime;

            if (waitingTime <= 0)
            {
                targetPosition = cat.transform.position + Random.insideUnitSphere * movingRadius;
                cat.NavMeshAgent.SetDestination(targetPosition);

                waitingTime = Random.Range(waitingIntervalMin, waitingIntervalMax);
            }
        }
        else
        {
            cat.Energy -= energyConsumption * Time.deltaTime;

            if (cat.Energy <= 0f)
            {
                cat.Energy = 0f;
                cat.ChangeState(cat.sleepingState);
            }
        }

        cat.Animator.SetFloat("Vert", cat.NavMeshAgent.velocity.magnitude / cat.NavMeshAgent.speed);
    }

    public override void OnCollisionEnter(StateManager cat, Collision collision)
    {
        
    }

    public override void ExitState(StateManager cat)
    {
        cat.NavMeshAgent.ResetPath();
    }
}
