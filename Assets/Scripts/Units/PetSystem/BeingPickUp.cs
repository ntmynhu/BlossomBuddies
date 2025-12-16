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
        toolHandler.OnPickupPet(cat);

        delayTime = maxDelayTime;
        StatsRate = cat.PetRateDict[PetStateType.BeingPickup];
    }

    public override void UpdateState(PetStateManager cat)
    {
        base.UpdateState(cat);

        if (delayTime < 0)
        {
            delayTime = -1;
        }
        else
        {
            delayTime -= Time.deltaTime;
        }
    }

    public override void OnInteract(PetStateManager cat)
    {
        base.OnInteract(cat);

        if (delayTime < 0)
        {
            cat.ChangeState(cat.walkAroundState);
        }
    }

    public override void ExitState(PetStateManager cat)
    {
        ToolHandler toolHandler = GameManager.Instance.Player.GetComponent<ToolHandler>();
        toolHandler.OnPutDownPet(cat);
    }
}
