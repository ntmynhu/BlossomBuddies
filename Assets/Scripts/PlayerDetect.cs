using System;
using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    [Tooltip("If true, the current interaction is store ")]
    [SerializeField] protected bool isInteractionActive = false;

    protected GameObject player;
    protected ToolHandler toolHandler;
    protected bool isPlayerInRange = false;

    public ToolHandler ToolHandler => toolHandler;
    public bool IsPlayerInRange => isPlayerInRange;
    public GameObject Player => player;

    public Action onPlayerEnter;
    public Action onPlayerExit; 

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            onPlayerEnter.Invoke();

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
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            onPlayerExit.Invoke();

            if (isInteractionActive)
            {
                toolHandler.SetCurrentInteraction(null);
            }
        }
    }
}
