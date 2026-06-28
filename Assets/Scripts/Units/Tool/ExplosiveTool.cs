using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTool : Tool
{
    [SerializeField] private List<ObjectData> targetObjects;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX());
    }

    public override void OnToolSelected(GameObject player)
    {
        base.OnToolSelected(player);
        List<string> ids = targetObjects.ConvertAll(o => o.Id);
        PlacementSystem.Instance.SwitchToExplosiveState(ids);
    }

    private IEnumerator PlayAnimationAndFX()
    {
        if (!PlacementSystem.Instance.CanTriggerAction())
        {
            yield break;
        }

        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.USE_TOOL);
        yield return new WaitForSeconds(0.5f);

        PlacementSystem.Instance.TriggerAction();

        playerMovement.SetMovementEnable(true);
    }
}
