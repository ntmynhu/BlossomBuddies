using System.Collections;
using UnityEngine;

public class WateringCan : Tool
{
    [SerializeField] private ObjectData spawnObject;
    [SerializeField] private ParticleSystem waterFX;
    [SerializeField] private float waterTime;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement, toolHandler));
    }

    public override void OnToolSelected(GameObject player)
    {
        base.OnToolSelected(player);
        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.WateringState, spawnObject);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement, ToolHandler toolHandler)
    {
        if (PlacementSystem.Instance.CanTriggerAction())
        {
            waterFX.Play();
            playerMovement.SetMovementEnable(false);
            playerAnim.PlayAnimation(playerAnim.INTERACT_LOOP);

            // Wait for the player to release the mouse button or for the water time to be up
            float waterTimer = waterTime;

            while (!Input.GetMouseButtonUp(0))
            {
                waterTimer -= Time.deltaTime;

                if (waterTimer <= 0f)
                {
                    PlacementSystem.Instance.TriggerAction();

                    playerMovement.SetMovementEnable(true);
                    playerAnim.PlayAnimation(playerAnim.INTERACT_BACK);
                    waterFX.Stop();

                    yield break;
                }

                yield return null;
            }

            playerMovement.SetMovementEnable(true);
            playerAnim.PlayAnimation(playerAnim.INTERACT_BACK);
            waterFX.Stop();
        }

        if (toolHandler.CurrentInteraction != null && toolHandler.CurrentInteraction.CompareTag("BathTub"))
        {
            BathTub bathTub = toolHandler.CurrentInteraction.GetComponent<BathTub>();

            if (bathTub.CurrentPet != null)
            {
                bathTub.CurrentPet.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
