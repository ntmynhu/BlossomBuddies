using UnityEngine;

public class HouseController : PlayerDetect
{
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        onPlayerEnter += OpenDoor;
        onPlayerExit += CloseDoor;
    }

    private void OnDisable()
    {
        onPlayerEnter -= OpenDoor;
        onPlayerExit -= CloseDoor;
    }

    private void OpenDoor()
    {
        Debug.Log("OpenDoor");
        animator.Play("OpenDoor");
    }

    private void CloseDoor()
    {
        Debug.Log("CloseDoor");
        animator.Play("CloseDoor");
    }
}
