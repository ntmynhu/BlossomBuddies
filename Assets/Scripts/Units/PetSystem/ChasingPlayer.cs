using UnityEngine;

public class ChasingPlayer : PetBaseState
{
    private Transform player;
    private float chaseSpeed = 3.5f;

    private PetStateManager cat;

    public override void EnterState(PetStateManager cat)
    {
        this.cat = cat;

        cat.NavMeshAgent.speed = chaseSpeed;
        player = GameManager.Instance.Player.transform;

        cat.Animator.SetFloat("State", 1);
        StatsRate = cat.PetRateDict[PetStateType.FindPlayer];

        GameEventManager.Instance.OnToyInteract += OnToyInteract;
    }

    public override void ExitState(PetStateManager cat)
    {
        cat.NavMeshAgent.ResetPath();

        GameEventManager.Instance.OnToyInteract -= OnToyInteract;
    }

    public override void UpdateState(PetStateManager cat)
    {
        base.UpdateState(cat);

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

    private void OnToyInteract()
    {
        if (cat.NavMeshAgent.velocity.magnitude <= 0.1f)
        {
            cat.Animator.Play("ToyInteract");
            cat.Happiness += 10f;

            if (cat.Happiness > 70f)
            {
                cat.ChangeState(cat.walkAroundState);
            }
        }
    }

    public override void OnTriggerEnter(PetStateManager cat, Collider other)
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
