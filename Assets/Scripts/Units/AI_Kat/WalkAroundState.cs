using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class WalkAroundState : BaseState
{
    private float waitingIntervalMin = 1f;
    private float waitingIntervalMax = 5f;

    private float waitingTime;

    private float movingRadius = 10f;
    private float movingAroundPlayerRadius = 2f;

    private Vector3 targetPosition;

    private float energyConsumption = 1f;
    private float happinessConsumption = 5f;
    private float foodConsumption = 5f;

    private float walkingSpeed = 1f;

    public override void EnterState(StateManager cat)
    {
        cat.NavMeshAgent.speed = walkingSpeed;
        waitingTime = 0;

        cat.Animator.SetFloat("State", 0);
    }

    public override void UpdateState(StateManager cat)
    {
        cat.Happiness -= happinessConsumption * Time.deltaTime;
        cat.Food -= foodConsumption * Time.deltaTime;

        float currentSpeed = cat.NavMeshAgent.velocity.magnitude;

        if (currentSpeed <= 0)
        {
            waitingTime -= Time.deltaTime;

            if (waitingTime <= 0)
            {
                if (cat.Food <= 0f)
                {
                    var foodPos = PetManager.Instance.FoodPosition.position;
                    targetPosition = foodPos;
                }
                else if (cat.Happiness <= 0f)
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

    public override void OnTriggerEnter(StateManager cat, Collider other)
    {
        if (other.CompareTag("Food") && cat.Food <= 0f)
        {
            cat.RunCoroutine(WaitToChangeState(cat));
        }

        if (other.CompareTag("Player"))
        {
            if (cat.Happiness <= 0f)
            {
                if (ToolManager.Instance.GetCurrentTool() is CatToy)
                {
                    cat.ChangeState(cat.chasingPlayerState);
                }
            }
            else if (cat.Happiness > 50)
            {
                cat.ChangeState(cat.runAwayFromPlayerState);
            }
        }
    }

    private IEnumerator WaitToChangeState(StateManager cat)
    {
        yield return new WaitUntil(() => Vector3.Distance(cat.transform.position, PetManager.Instance.FoodPosition.position) <= 0.5f);

        cat.ChangeState(cat.eatingState);
    }
}
