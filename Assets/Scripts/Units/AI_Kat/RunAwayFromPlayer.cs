using UnityEngine;

public class RunAwayFromPlayer : BaseState
{
    private Transform player;
    private float chaseSpeed = 3.5f;

    private float runAwayDistance = 5f;
    private float ranDomOffset = 1f;

    private float timeToChangeState = 5f;
    private float countTime = 0;

    private StateManager cat;

    public override void EnterState(StateManager cat)
    {
        this.cat = cat;
        countTime = 0;

        cat.NavMeshAgent.speed = chaseSpeed;
        player = GameManager.Instance.Player.transform;

        cat.NavMeshAgent.SetDestination(GetPositionAwayFromPlayer());

        cat.Animator.SetFloat("State", 1);
    }

    public override void ExitState(StateManager cat)
    {
        cat.NavMeshAgent.ResetPath();
    }

    public override void UpdateState(StateManager cat)
    {
        if (cat.NavMeshAgent.velocity.magnitude <= 0.1f)
        {
            countTime += Time.deltaTime;

            if (countTime > timeToChangeState)
            {
                cat.ChangeState(cat.walkAroundState);
            }    

            if (cat.Happiness <= 0)
            {
                cat.ChangeState(cat.walkAroundState);
            }    
        }

        cat.Animator.SetFloat("Vert", cat.NavMeshAgent.velocity.magnitude / cat.NavMeshAgent.speed);
    }

    public override void OnTriggerStay(StateManager cat, Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 targetPosition = GetPositionAwayFromPlayer();
            cat.NavMeshAgent.SetDestination(targetPosition);

            countTime = 0;
        }
    }

    private Vector3 GetPositionAwayFromPlayer()
    {
        Vector3 directionAway = (cat.transform.position - player.position).normalized;

        directionAway += new Vector3(Random.Range(-ranDomOffset, ranDomOffset), 0, Random.Range(-ranDomOffset, ranDomOffset));

        return cat.transform.position + directionAway * runAwayDistance;
    }
}
