using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class WalkAroundState : PetBaseState
{
    #region Fields
    private float waitingIntervalMin = 1f;
    private float waitingIntervalMax = 5f;

    private float waitingTime;

    private float movingRadius = 10f;
    private float movingAroundPlayerRadius = 2f;

    private Vector3 targetPosition;
    private float walkingSpeed = 1f;
    #endregion

    public override void EnterState(PetStateManager cat)
    {
        cat.NavMeshAgent.speed = walkingSpeed;
        waitingTime = 0;

        cat.Animator.SetFloat("State", 0);

        StatsRate = cat.PetRateDict[PetStateType.WalkAround];
    }

    public override void UpdateState(PetStateManager cat)
    {
        base.UpdateState(cat);

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

        if (cat.Energy <= 0f)
        {
            cat.Energy = 0f;
            cat.ChangeState(cat.sleepingState);
        }

        cat.Animator.SetFloat("Vert", cat.NavMeshAgent.velocity.magnitude / cat.NavMeshAgent.speed);
    }

    private Vector3 GetNextTargetPosition(Vector3 centerPosition, float radius)
    {
        return centerPosition + Random.insideUnitSphere * radius;
    }

    public override void OnCollisionEnter(PetStateManager cat, Collision collision)
    {
        
    }

    public override void ExitState(PetStateManager cat)
    {
        cat.NavMeshAgent.ResetPath();
    }

    public override void OnTriggerEnter(PetStateManager cat, Collider other)
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

    private IEnumerator WaitToChangeState(PetStateManager cat)
    {
        yield return new WaitUntil(() => Vector3.Distance(cat.transform.position, PetManager.Instance.FoodPosition.position) <= 0.5f);

        cat.ChangeState(cat.eatingState);
    }
}
