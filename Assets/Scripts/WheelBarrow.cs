using UnityEngine;
using UnityEngine.UIElements;

public class WheelBarrow : PlayerDetect
{
    [SerializeField] private Transform spawnPos;

    private ToolHandler toolHandler;
    private bool isAttachedToPlayer = false;

    private void Start()
    {
        player = GameManager.Instance.Player;
        toolHandler = player.GetComponent<ToolHandler>();
        StartCoroutine(ToolManager.Instance.InitializeTools(spawnPos));
    }

    private void Update()
    {
        if (isPlayerInRange && !isAttachedToPlayer && Input.GetKeyDown(KeyCode.E))
        {
            AttachToPlayer();
        }
        else if (isAttachedToPlayer && Input.GetKeyDown(KeyCode.E))
        {
            DetachFromPlayer();
        }

        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                toolHandler.SelectTool(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                toolHandler.SelectTool(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                toolHandler.SelectTool(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                toolHandler.UnSelectTool();
            }
        }
    }

    private void AttachToPlayer()
    {
        isAttachedToPlayer = true;

        var parentTransform = player.transform.Find("WheelBarrowTransform");
        this.transform.SetParent(parentTransform);

        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
    }

    private void DetachFromPlayer()
    {
        isAttachedToPlayer = false;

        this.transform.SetParent(null);
        this.transform.position = player.transform.position + player.transform.forward * 2f; // Place it in front of the player
        this.transform.rotation = Quaternion.identity; // Reset rotation
    }
}
