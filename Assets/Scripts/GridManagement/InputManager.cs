using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask placementLayermask;

    private Vector3 lastPosition;

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    public Vector3 GetPlayerSelectedMapPosition()
    {
        Vector3 playerPos = player.transform.position + Vector3.up * 1f;
        if (Physics.Raycast(playerPos, Vector3.down, out RaycastHit hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
            Debug.Log("Hit");
        }
        return lastPosition;
    }
}
