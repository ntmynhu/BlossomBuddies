using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IDataPersistence
{
    [SerializeField] private int timeScale = 1;

    private GameObject player;
    private PlayerMovement playerMovement;
    private PlayerAnimation playerAnimation;
    private ToolHandler toolHandler;
    private int currentHeart;

    private SceneName currentScene;

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
    public SceneName CurrentScene => currentScene;
    #endregion

    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMusicClip);
        Time.timeScale = timeScale;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;

        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimation = player.GetComponent<PlayerAnimation>();
        toolHandler = player.GetComponent<ToolHandler>();
    }

    public void AddHeart(int value)
    {
        this.CurrentHeart += value;

        playerAnimation.PlayHeartAnimation();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.heartSoundClip);
    }

    public void SetMovementFrozen(bool frozen)
    {
        if (playerMovement != null)
            playerMovement.SetMovementEnable(!frozen);
    }

    public void SetCurrentScene(SceneName scene)
    {
        currentScene = scene;
    }

    public void LoadData(GameData data)
    {
        this.currentHeart = data.currentHeart;
    }

    public void SaveData(ref GameData data)
    {
        data.currentHeart = this.currentHeart;
    }
}

[Serializable]
public enum SceneName
{
    MainScene,
    HouseInterior,
}