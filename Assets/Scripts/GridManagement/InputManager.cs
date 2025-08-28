using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;

    private Vector3 lastPosition;
    private GameObject player;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

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
        Vector3 playerPos = player.transform.position + player.transform.forward/2 + Vector3.up * 1f;
        if (Physics.Raycast(playerPos, Vector3.down, out RaycastHit hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
