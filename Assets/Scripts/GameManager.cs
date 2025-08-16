using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject player;
    public GameObject Player => player;
}
