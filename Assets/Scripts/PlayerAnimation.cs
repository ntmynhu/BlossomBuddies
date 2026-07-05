using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Animator heartAnim;

    public readonly string USE_TOOL = "UseTool";
    public readonly string INTERACT = "Interact";
    public readonly string INTERACT_LOOP = "Interact_Loop";
    public readonly string INTERACT_BACK = "Interact_Back";

    public void PlayAnimation(string animationName)
    {
        playerAnim.Play(animationName);
    }

    public void PlayHeartAnimation()
    {
        heartAnim.transform.LookAt(Camera.main.transform);
        heartAnim.Play("Heart");
    }
}
