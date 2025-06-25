using UnityEngine;
using UnityEngine.UIElements;

public class WheelBarrow : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float detectionRadius;

    private float distanceToPlayer;
    private GameObject player;

    private bool isPlayerInRange = false;
    private bool isAttachedToPlayer = false;

    private void Start()
    {
        player = GameManager.Instance.Player;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
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
