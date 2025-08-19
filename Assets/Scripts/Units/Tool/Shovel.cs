using System.Collections;
using UnityEngine;

public class Shovel : Tool
{
    [SerializeField] private ObjectData spawnObject;

    public override void UseTool()
    {
        StartCoroutine(PlayAnimationAndFX(playerAnim, playerMovement));
    }

    public override void OnToolSelected(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        base.OnToolSelected(playerAnim, playerMovement);
        
        PlacementSystem.Instance.SetCurrentSelectedIndex(spawnObject.ID);
    }

    private IEnumerator PlayAnimationAndFX(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        if (!PlacementSystem.Instance.CanPlaceAt())
        {
            Debug.Log("Cannot place object at the current position.");
            yield break;
        }

        playerMovement.SetMovementEnable(false);
        playerAnim.PlayAnimation(playerAnim.USE_TOOL);
        yield return new WaitForSeconds(0.5f);

        PlacementSystem.Instance.PlaceObject();

        playerMovement.SetMovementEnable(true);
    }
}
