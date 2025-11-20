using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    protected GameObject player;
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
        }

        Debug.Log("PlayerDetect");
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }

        Debug.Log("PlayerDetect Exit");
    }
}
