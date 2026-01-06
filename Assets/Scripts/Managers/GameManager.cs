using UnityEngine;

public class GameManager : Singleton<GameManager>, IDataPersistence
{
    [SerializeField] private int timeScale = 1;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator heartAnim;

    private PlayerMovement playerMovement;
    private ToolHandler toolHandler;
    private int currentHeart;

    #region Properties
    public GameObject Player => player;
    public PlayerMovement PlayerMovement => playerMovement;
    public ToolHandler ToolHandler => toolHandler;
    public int CurrentHeart
    {
        get => currentHeart;
        set
        {
            currentHeart = value;
            GameEventManager.Instance.TriggerHeartNumberChange();
        }
    }
    #endregion

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        toolHandler = player.GetComponent<ToolHandler>();
        Time.timeScale = timeScale;
    }

    public void AddHeart(int value)
    {
        this.CurrentHeart += value;

        heartAnim.transform.LookAt(Camera.main.transform);
        heartAnim.Play("Heart");
    }

    public void LoadData(GameData data)
    {
        this.CurrentHeart = data.currentHeart;
    }

    public void SaveData(ref GameData data)
    {
        data.currentHeart = this.CurrentHeart;
    }
}
