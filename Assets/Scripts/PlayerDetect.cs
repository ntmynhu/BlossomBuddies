using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    [SerializeField] protected bool isInteractionActive = false;

    protected GameObject player;
    protected ToolHandler toolHandler;
    protected bool isPlayerInRange = false;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (player == null)
            {
                player = other.gameObject;
            }

            if (toolHandler == null)
            {
                toolHandler = player.GetComponent<ToolHandler>();
            }

            if (isInteractionActive)
            {
                toolHandler.SetCurrentInteraction(gameObject);
            }
            
        }

        Debug.Log("PlayerDetect");
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (isInteractionActive)
            {
                toolHandler.SetCurrentInteraction(null);
            }
        }

        Debug.Log("PlayerDetect Exit");
    }
}
