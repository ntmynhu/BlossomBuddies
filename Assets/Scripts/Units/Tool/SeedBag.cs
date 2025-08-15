using System.Collections;
using UnityEngine;

public class SeedBag : Tool
{
    [SerializeField] private PlantData spawnObject;

    public override void UseTool(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement));
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.USE_TOOL);
        yield return new WaitForSeconds(0.5f);
        PlacementSystem.Instance.PlaceObject(spawnObject.ID);
        playerMovement.SetMovementEnable(true);
    }
}
