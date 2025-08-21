using UnityEngine;

public class ChasingPlayer : BaseState
{
    private Transform player;
    private float chaseSpeed = 3.5f;

    public override void EnterState(StateManager cat)
    {
        cat.NavMeshAgent.speed = chaseSpeed;
        player = GameManager.Instance.Player.transform;

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
            Vector3 targetPosition = player.position + (player.forward);
            cat.NavMeshAgent.SetDestination(targetPosition);
        }
        else
        {
            cat.transform.LookAt(player.position);
        }

        cat.Animator.SetFloat("Vert", cat.NavMeshAgent.velocity.magnitude / cat.NavMeshAgent.speed);
    }

    public override void OnTriggerEnter(StateManager cat, Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ToolManager.Instance.GetCurrentTool() is not CatToy)
            {
                cat.ChangeState(cat.walkAroundState);
            }
        }
    }
}
