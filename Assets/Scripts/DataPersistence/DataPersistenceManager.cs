using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using BlossomBuddies.Network;
using UnityEngine;

public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    private string dataKey = "gameData";
    private bool useEncryption = false;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private PlayerPrefsDataHandler dataHandler;

    private long lastLoginTime = DateTime.MinValue.Ticks;
    public long LastLoginTime => lastLoginTime;

    public bool isLoadedDataDone = false;

    // True when the logged-in account had no cloud save yet (brand-new account). Used to seed
    // default inventory (gardening tools) into the server instead of pulling an empty one.
    public bool IsNewAccount { get; private set; }

    //Test
    public float hoursSinceLastLogin = 0f;

    private void OnEnable()
    {
        // Only prepare the local (offline cache) handler here. The real load happens after
        // login via LoadFromServer, so we never show one account's garden to another.
        if (this.dataHandler == null)
            this.dataHandler = new PlayerPrefsDataHandler(dataKey, useEncryption);
    }

    public void InitAndLoadGame() //void Start
    {
        this.dataHandler = new PlayerPrefsDataHandler(dataKey, useEncryption);
        //is loading -> has not have scene -> cannot call
        //this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    /// <summary>
    /// Loads the player's progress from the server cloud save (per account). A brand-new
    /// account has no save yet, so it starts from defaults. Falls back to the local cache
    /// when offline. Calls <paramref name="onComplete"/> once gameData is ready.
    /// </summary>
    public void LoadFromServer(Action onComplete)
    {
        if (this.dataHandler == null)
            this.dataHandler = new PlayerPrefsDataHandler(dataKey, useEncryption);

        bool loggedIn = SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn;
        if (!loggedIn)
        {
            // Offline / not signed in: use local cache or defaults.
            IsNewAccount = false;
            ApplyLoaded(this.dataHandler.Load(), cacheLocally: false);
            onComplete?.Invoke();
            return;
        }

        GameApi.Save.Get(
            payload =>
            {
                GameData data = null;
                bool hasCloud = payload != null && !string.IsNullOrEmpty(payload.Json);
                if (hasCloud)
                {
                    try { data = JsonUtility.FromJson<GameData>(payload.Json); }
                    catch (Exception e) { Debug.LogError("[Save] Failed to parse cloud save: " + e); }
                }
                // No cloud save yet == brand-new account -> defaults (and seed default inventory).
                IsNewAccount = !hasCloud;
                ApplyLoaded(data, cacheLocally: true);
                onComplete?.Invoke();
            },
            err =>
            {
                Debug.LogWarning("[Save] Cloud load failed, using local cache/defaults: " + err);
                IsNewAccount = false;
                ApplyLoaded(this.dataHandler.Load(), cacheLocally: false);
                onComplete?.Invoke();
            });
    }

    private void ApplyLoaded(GameData data, bool cacheLocally)
    {
        this.gameData = data ?? new GameData();

        if (hoursSinceLastLogin > 0)
            this.gameData.lastLoginTime -= (long)(hoursSinceLastLogin * 3600f * 10000000f);

        lastLoginTime = this.gameData.lastLoginTime;

        // Keep the local cache in sync with the account we just loaded (prevents another
        // account's stale cache from leaking into scene transitions before the first save).
        if (cacheLocally)
            this.dataHandler.Save(this.gameData);

        PushLoadedDataToObject();
    }

    const float SAVE_INTERVAL = 30f;
    float timeSave = SAVE_INTERVAL;
    private void Update()
    {
        timeSave -= Time.deltaTime;
        if (timeSave <= 0)
        {
            SaveGame();
            timeSave = SAVE_INTERVAL;
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // load any saved data from a file using data handler
        this.gameData = dataHandler.Load();

        // if there is no data to load
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        if (hoursSinceLastLogin > 0)
        {
            this.gameData.lastLoginTime -= (long)(hoursSinceLastLogin * 3600f * 10000000f);
        }

        lastLoginTime = this.gameData.lastLoginTime;

        // push the loaded data to all other scripts that need it
        PushLoadedDataToObject();
    }

    public void PushLoadedDataToObject()
    {
        UpdateAllDataPersistenceObjects();

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            Debug.Log("Load: " + dataPersistenceObj);
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SetLoadedDataDone()
    {
        isLoadedDataDone = true;
        Debug.Log("Loaded done");
    }

    public void SaveGame()
    {
        if (isLoadedDataDone /*&& !GameManager.Instance.IsFirstTimePlayer()*/)
        {
            gameData.lastLoginTime = DateTime.Now.Ticks;

            UpdateAllDataPersistenceObjects();

            // save the data in all other scripts that need to save data
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                MonoBehaviour mono = dataPersistenceObj as MonoBehaviour;

                if (mono.gameObject.scene.name == GameManager.Instance.CurrentScene.ToString())
                {
                    dataPersistenceObj.SaveData(ref gameData);
                }
            }

            // save the data to a local file (offline cache)
            dataHandler.Save(gameData);

            // and push to the server cloud save for this account
            if (SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn)
            {
                GameApi.Save.Push(JsonUtility.ToJson(gameData),
                    _ => { },
                    err => Debug.LogWarning("[Save] Cloud push failed: " + err));
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (isLoadedDataDone && pauseStatus)
        {
            SaveGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (isLoadedDataDone && !hasFocus)
        {
            SaveGame();
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void UpdateAllDataPersistenceObjects()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        Debug.Log("Update");
    }
}
