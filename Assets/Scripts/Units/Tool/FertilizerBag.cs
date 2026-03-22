using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerBag : Tool
{
    [SerializeField] private ObjectData spawnObject;
    [SerializeField] private ParticleSystem waterFX;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement, toolHandler));
    }

    public override void OnToolSelected(GameObject player)
    {
        base.OnToolSelected(player);
        PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.FertilizingState, spawnObject);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement, ToolHandler toolHandler)
    {
        if (!PlacementSystem.Instance.CanTriggerAction())
        {
            Debug.Log("Cannot place object at the current position.");
            yield break;
        }

        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.USE_TOOL);
        yield return new WaitForSeconds(0.5f);

        PlacementSystem.Instance.TriggerAction();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.shovelSoundClip);

        playerMovement.SetMovementEnable(true);
    }
}
