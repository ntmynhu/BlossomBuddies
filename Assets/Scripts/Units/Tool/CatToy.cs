using UnityEngine;

public class CatToy : Tool
{
    public override void UseTool()
    {
        // Implement the logic for using the cat toy
        Debug.Log("Using the cat toy!");
    }

    public override void OnToolSelected(PlayerAnimation playerAnim, PlayerMovement playerMovement)
    {
        base.OnToolSelected(playerAnim, playerMovement);

        // Additional logic when the cat toy is selected
        Debug.Log("Cat toy selected!");
    }
}
