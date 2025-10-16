using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.AI;

public class BeingPickUp : BaseState
{
    float maxDelayTime = 0.5f;
    float delayTime;

    public override void EnterState(StateManager cat)
    {
        ToolHandler toolHandler = GameManager.Instance.Player.GetComponent<ToolHandler>();

        cat.NavMeshAgent.enabled = false;
        cat.Rigidbody.isKinematic = true;
        cat.transform.SetParent(toolHandler.PetTransform);

        cat.transform.localPosition = Vector3.zero;
        cat.transform.localRotation = Quaternion.identity;

        delayTime = maxDelayTime;
    }

    public override void UpdateState(StateManager cat)
    {
        if (delayTime < 0)
        {
            Debug.Log("Waiting for input");
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

    public override void ExitState(StateManager cat)
    {
        var player = GameManager.Instance.Player;
        Vector3 targetPos = player.transform.position + player.transform.forward;

        cat.transform.position = targetPos;
        cat.transform.SetParent(null);

        cat.NavMeshAgent.enabled = true;
        cat.Rigidbody.isKinematic = false;
    }
}
