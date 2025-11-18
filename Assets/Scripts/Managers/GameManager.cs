using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int timeScale = 1;
    [SerializeField] private GameObject player;

    private PlayerMovement playerMovement;
    public GameObject Player => player;
    public PlayerMovement PlayerMovement => playerMovement;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        Time.timeScale = timeScale;
        Debug.Log("GameManager started with timeScale: " + timeScale);
    }
}
