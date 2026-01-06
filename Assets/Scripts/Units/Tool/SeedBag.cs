using System.Collections;
using UnityEngine;

public class SeedBag : Tool
{
    [SerializeField] private PlantData spawnObject;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement));
    }

    public override void OnToolSelected(GameObject player)
    {
        base.OnToolSelected(player);

        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.PlantState, spawnObject);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        if (!PlacementSystem.Instance.CanTriggerAction())
        {
            Debug.Log("Cannot plant at the current position.");
            yield break;
        }

        Debug.Log(playerMovement);

        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.USE_TOOL);

        yield return new WaitForSeconds(0.5f);

        PlacementSystem.Instance.TriggerAction();
        playerMovement.SetMovementEnable(true);
    }
}
