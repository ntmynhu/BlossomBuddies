using System.Collections;
using UnityEngine;

public class PetSoap : Tool
{
    float duration = 5f;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement, toolHandler));
    }

    public override void OnToolSelected(GameObject player)
    {
        base.OnToolSelected(player);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement, ToolHandler toolHandler)
    {
        PetStateHandler targetPet = null;
        BathingState bathingState = null;

        if (toolHandler.CurrentInteraction == null)
        {
            yield break;
        }

        if (toolHandler.CurrentInteraction.CompareTag("BathTub") == false)
        {
            Debug.Log("No Interact with Bathtub.");
            yield break;
        }

        if (toolHandler.CurrentInteraction.CompareTag("BathTub"))
        {
            BathTub bathTub = toolHandler.CurrentInteraction.GetComponent<BathTub>();
            if (bathTub.CurrentPet == null)
            {
                Debug.Log("No pet in the bathtub.");
                yield break;
            }
            else
            {
                targetPet = bathTub.CurrentPet;
                bathingState = targetPet.bathingState as BathingState;
            }
        }

        if (bathingState == null)
        {
            Debug.Log("Target pet is not in bathing state.");
            yield break;
        }

        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.INTERACT_LOOP);

        // Wait for the player to release the mouse button or for the water time to be up
        float soapTimer = duration;

        while (!Input.GetMouseButtonUp(0))
        {
            soapTimer -= Time.deltaTime;
            bathingState.OnSoapInteract(targetPet);

            //if (bathingState.IsShowAllBubbles)
            //{
            //    playerMovement.SetMovementEnable(true);
            //    playerAnim.PlayAnimation(playerAnim.INTERACT_BACK);

            //    yield break;
            //}

            yield return null;
        }

        playerMovement.SetMovementEnable(true);
        playerAnim.PlayAnimation(playerAnim.INTERACT_BACK);
    }
}
