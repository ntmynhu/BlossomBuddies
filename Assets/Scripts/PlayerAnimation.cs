using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;

    public readonly string USE_TOOL = "UseTool";

    public void PlayAnimation(string animationName)
    {
        playerAnim.Play(animationName);
    }
}
