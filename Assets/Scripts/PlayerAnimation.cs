using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;

    public readonly string USE_TOOL = "UseTool";
    public readonly string INTERACT = "Interact";

    public void PlayAnimation(string animationName)
    {
        playerAnim.Play(animationName);
    }
}
