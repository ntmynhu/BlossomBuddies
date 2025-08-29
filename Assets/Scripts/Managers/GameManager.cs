using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject player;
    public GameObject Player => player;

    private PlayerMovement playerMovement;
    public PlayerMovement PlayerMovement => playerMovement;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
    }
}
