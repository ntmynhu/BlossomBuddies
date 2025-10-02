using System.Collections;
using UnityEngine;

public class WateringCan : Tool
{
    [SerializeField] private ObjectData spawnObject;
    [SerializeField] private ParticleSystem waterFX;
    [SerializeField] private float waterTime;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement));
    }

    public override void OnToolSelected(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        base.OnToolSelected(playerAnim, playerMovement);
        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.WateringState, spawnObject);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        //if (!PlacementSystem.Instance.CanTriggerAction())
        //{
        //    yield break;
        //}

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
}
