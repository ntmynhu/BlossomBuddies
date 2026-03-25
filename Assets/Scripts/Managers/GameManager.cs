using UnityEngine;

public class GameManager : Singleton<GameManager>, IDataPersistence
{
    [SerializeField] private int timeScale = 1;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator heartAnim;

    private PlayerMovement playerMovement;
    private ThirdPersonCameraController thirdPersonCameraController;
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
        AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMusicClip);

        playerMovement = player.GetComponent<PlayerMovement>();
        thirdPersonCameraController = player.GetComponent<ThirdPersonCameraController>();
        toolHandler = player.GetComponent<ToolHandler>();
        Time.timeScale = timeScale;
    }

    public void AddHeart(int value)
    {
        this.CurrentHeart += value;

        heartAnim.transform.LookAt(Camera.main.transform);
        heartAnim.Play("Heart");

        AudioManager.Instance.PlaySFX(AudioManager.Instance.heartSoundClip);
    }

    public void SetCameraFrozen(bool value)
    {
        thirdPersonCameraController.SetMobileController(value);
        thirdPersonCameraController.SetCameraFrozen(value);
        playerMovement.SetMovementEnable(!value);
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
