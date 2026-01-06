using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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

    //Test
    public float hoursSinceLastLogin = 0f;

    private void OnEnable()
    {
        InitAndLoadGame();
    }

    public void InitAndLoadGame() //void Start
    {
        this.dataHandler = new PlayerPrefsDataHandler(dataKey, useEncryption);
        //is loading -> has not have scene -> cannot call
        //this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
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
                dataPersistenceObj.SaveData(ref gameData);
            }

            // save the data to a file using data handler
            dataHandler.Save(gameData);
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
