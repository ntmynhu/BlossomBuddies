using UnityEngine;

public class WalkAroundState : BaseState
{
    private float waitingIntervalMin = 1f;
    private float waitingIntervalMax = 5f;

    private float waitingTime;

    private float movingRadius = 10f;
    private float movingAroundPlayerRadius = 5f;

    private Vector3 targetPosition;

    private float energyConsumption = 10f;
    private float happinessConsumption = 5f;

    public override void EnterState(StateManager cat)
    {
        waitingTime = Random.Range(waitingIntervalMin, waitingIntervalMax);
    }

    public override void UpdateState(StateManager cat)
    {
        if (cat.Happiness > 0)
        {
            cat.Happiness -= happinessConsumption * Time.deltaTime;

            if (cat.Happiness <= 0f)
            {
                cat.Happiness = 0f;
            }
        }

        float currentSpeed = cat.NavMeshAgent.velocity.magnitude;

        if (currentSpeed <= 0)
        {
            waitingTime -= Time.deltaTime;

            if (waitingTime <= 0)
            {
                if (cat.Happiness <= 0f)
                {
                    var playerPos = GameManager.Instance.Player.transform.position;
                    targetPosition = GetNextTargetPosition(playerPos, movingAroundPlayerRadius);
                }
                else
                {
                    targetPosition = GetNextTargetPosition(cat.transform.position, movingRadius);
                }

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

    private Vector3 GetNextTargetPosition(Vector3 centerPosition, float radius)
    {
        return centerPosition + Random.insideUnitSphere * radius;
    }

    public override void OnCollisionEnter(StateManager cat, Collision collision)
    {
        
    }

    public override void ExitState(StateManager cat)
    {
        cat.NavMeshAgent.ResetPath();
    }
}
