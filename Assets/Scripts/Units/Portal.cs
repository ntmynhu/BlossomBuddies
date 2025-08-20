using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal portalOut;
    [SerializeField] private Transform outPosition;

    public Transform OutPosition => outPosition;

    private void OnTriggerEnter(Collider other)
    {
        Transform targetTransform = portalOut.OutPosition;

        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerMovement.SetPosition(targetTransform);
        }
    }
}
