using UnityEngine;

public class CatWalkAroundState : CatBaseState
{
    private float waitingIntervalMin = 1f;
    private float waitingIntervalMax = 5f;

    private float waitingTime;

    private float movingRadius = 10f;
    private Vector3 targetPosition;

    public override void EnterState(CatStateManager cat)
    {
        waitingTime = Random.Range(waitingIntervalMin, waitingIntervalMax);
    }

    public override void UpdateState(CatStateManager cat)
    {
        if (waitingTime > 0f)
        {
            if (HasReachedDestination(cat))
            {
                waitingTime -= Time.deltaTime;
            }
        }
        else
        {
            Vector3 randomPosition = cat.transform.position + Random.insideUnitSphere * movingRadius;
            cat.NavMeshAgent.SetDestination(randomPosition);

            waitingTime = Random.Range(waitingIntervalMin, waitingIntervalMax);
        }

        cat.Animator.SetFloat("Vert", cat.NavMeshAgent.velocity.magnitude / cat.NavMeshAgent.speed);
    }

    private bool HasReachedDestination(CatStateManager cat)
    {
        return !cat.NavMeshAgent.pathPending && cat.NavMeshAgent.remainingDistance <= cat.NavMeshAgent.stoppingDistance;
    }

    public override void OnCollisionEnter(CatStateManager cat, Collision collision)
    {
        
    }
}
