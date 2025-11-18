using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.AI;

public class BeingPickUp : PetBaseState
{
    float maxDelayTime = 0.5f;
    float delayTime;

    public override void EnterState(PetStateManager cat)
    {
        ToolHandler toolHandler = GameManager.Instance.Player.GetComponent<ToolHandler>();

        cat.NavMeshAgent.enabled = false;
        cat.Rigidbody.isKinematic = true;
        cat.transform.SetParent(toolHandler.PetTransform);

        cat.transform.localPosition = Vector3.zero;
        cat.transform.localRotation = Quaternion.identity;

        delayTime = maxDelayTime;
        StatsRate = cat.PetRateDict[PetStateType.BeingPickup];
    }

    public override void UpdateState(PetStateManager cat)
    {
        base.UpdateState(cat);

        if (delayTime < 0)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                cat.ChangeState(cat.walkAroundState);
            }
        }
        else
        {
            delayTime -= Time.deltaTime;
        }
    }

    public override void ExitState(PetStateManager cat)
    {
        var player = GameManager.Instance.Player;
        Vector3 targetPos = player.transform.position + player.transform.forward;

        cat.transform.position = targetPos;
        cat.transform.SetParent(null);

        cat.NavMeshAgent.enabled = true;
        cat.Rigidbody.isKinematic = false;
    }
}
