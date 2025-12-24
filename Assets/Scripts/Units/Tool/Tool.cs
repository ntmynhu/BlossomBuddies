using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField] private ToolInfo toolInfo;
    [SerializeField] private Vector3 initialPos;
    [SerializeField] private Quaternion initialRot;

    private Rigidbody rb;

    public ToolInfo ToolInfo { get => toolInfo; set => toolInfo = value; }
    public Vector3 InitialPos { get => initialPos; set => initialPos = value; }
    public Quaternion InitialRot { get => initialRot; set => initialRot = value; }
    public Rigidbody Rigidbody => rb;

    protected GameObject player;
    protected PlayerAnimation playerAnim;
    protected PlayerMovement playerMovement;
    protected ToolHandler toolHandler;

    private void Awake()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
        rb = GetComponent<Rigidbody>();
    }

    public virtual void UseTool() { }
    public virtual void OnToolSelected(GameObject player)
    {
        this.player = player;
        playerAnim = player.GetComponent<PlayerAnimation>();
        playerMovement = player.GetComponent<PlayerMovement>();
        toolHandler = player.GetComponent<ToolHandler>();
    }
    public virtual void OnToolDeselected() { }
}
