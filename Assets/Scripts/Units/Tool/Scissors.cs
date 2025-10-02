using System.Collections;
using UnityEngine;

public class Scissors : Tool
{
    [SerializeField] private PlantData plantData;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement));
    }

    public override void OnToolSelected(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        base.OnToolSelected(playerAnim, playerMovement);
        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.ScissorsState, plantData);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.USE_TOOL);
        yield return new WaitForSeconds(0.5f);

        PlacementSystem.Instance.TriggerAction();

        playerMovement.SetMovementEnable(true);
    }
}
