using System.Collections;
using UnityEngine;

public class SeedBag : Tool
{
    [SerializeField] private PlantData spawnObject;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement));
    }

    public override void OnToolSelected(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        base.OnToolSelected(playerAnim, playerMovement);

        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.PlantState, spawnObject);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        if (!PlacementSystem.Instance.CanTriggerAction())
        {
            Debug.Log("Cannot plant at the current position.");
            yield break;
        }

        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.USE_TOOL);

        yield return new WaitForSeconds(0.5f);

        PlacementSystem.Instance.TriggerAction();
        playerMovement.SetMovementEnable(true);
    }
}
