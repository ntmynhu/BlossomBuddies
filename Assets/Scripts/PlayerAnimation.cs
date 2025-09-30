using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;

    public readonly string USE_TOOL = "UseTool";
    public readonly string INTERACT = "Interact";
    public readonly string INTERACT_LOOP = "Interact_Loop";
    public readonly string INTERACT_BACK = "Interact_Back";

    public void PlayAnimation(string animationName)
    {
        playerAnim.Play(animationName);
    }
}
