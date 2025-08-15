using UnityEngine;

public class Tool : MonoBehaviour
{
    public ToolInfo toolInfo;

    public Vector3 initialPos;
    public Quaternion initialRot;

    private void Awake()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
    }

    public virtual void UseTool(PlayerAnimation playerAnim, PlayerMovement playerMovement) { }
}
