using System.Collections;
using UnityEngine;

public class CatToy : Tool
{
    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement));
    }

    public override void OnToolSelected(GameObject player)
    {
        base.OnToolSelected(player);

        // Additional logic when the cat toy is selected
        Debug.Log("Cat toy selected!");
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.INTERACT);
        yield return new WaitForSeconds(0.5f);

        GameEventManager.Instance.TriggerToyInteract();

        playerMovement.SetMovementEnable(true);
    }
}
