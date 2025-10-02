using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private PlantData plantData;
    [SerializeField] private PlantStats plantStats;
    [SerializeField] private List<GameObject> stateGameObjects;

    [SerializeField] private ObjectData wateredSoilData;
    [SerializeField] private ObjectData wateredFadeOutSoilData;

    [SerializeField] private List<GameObject> grassList;

    private Vector3Int mainPosition;
    private float growthTime = 0;
    private int currentStateIndex = 0;
    private float currentStateTime = 0;

    private bool isDead = false;

    #region Watering Variables
    private float waterTimer;
    private int waterState;
    private bool isWatered = false;
    #endregion

    #region Grass Variables
    private float tickTimer = 0;
    #endregion

    #region Properties
    public Vector3Int MainPosition { get => mainPosition; set => mainPosition = value; }
    public ObjectData WateredSoilData => wateredSoilData;
    public ObjectData WateredFadeOutSoilData => wateredFadeOutSoilData;
    public bool IsDead => isDead;
    public bool IsFullyGrown => currentStateIndex == plantData.plantStates.Count - 2; // Last index is dead state
    public bool IsWeeded => grassList.Exists(g => g.activeInHierarchy);
    #endregion

    private void Start()
    {
        tickTimer = plantStats.WEED_TICK_TIME;
        currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;
    }

    private void Update()
    {
        if (isWatered)
        {
            HandleWaterLevel();
        }

        if (isDead)
        {
            return;
        }

        // Handle Plant Growth
        growthTime += Time.deltaTime;

        // Apply Water Bonus
        if (isWatered)
        {
            growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * Time.deltaTime;
        }

        // Apply Weed Penalty
        float totalPenalty = 0;
        int multiplier = isWatered ? plantStats.WATER_MULTIPLIER : 1;
        foreach (var grass in grassList)
        {
            if (grass.activeInHierarchy)
            {
                totalPenalty += plantStats.WEED_PENALTY_SPEED * Time.deltaTime * grass.transform.localScale.x * multiplier;
            }
        }

        growthTime -= totalPenalty;

        if (growthTime >= currentStateTime)
        {
            AdvanceToNextState();
        }
        //

        // Handle Grass Spawn
        tickTimer -= Time.deltaTime;
        if (tickTimer < 0)
        {
            tickTimer = plantStats.WEED_TICK_TIME;
            CheckGrassSpawn();
        }

        HandleGrassGrowth();
        //
    }

    #region Handle Flower State
    private void AdvanceToNextState()
    {
        growthTime = 0;
        currentStateIndex++;

        currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;
        UpdatePlantStateVisual();

        if (currentStateIndex >= plantData.plantStates.Count - 1)
        {
            isDead = true;
            Debug.Log("Plant has died.");
        }
    }

    private void UpdatePlantStateVisual()
    {
        if (currentStateIndex < 0 || currentStateIndex >= stateGameObjects.Count)
        {
            Debug.LogError("Current state index is out of bounds: " + currentStateIndex);
            return;
        }

        for (int i = 0; i < stateGameObjects.Count; i++)
        {
            stateGameObjects[i].SetActive(i == currentStateIndex);
        }
    }
    #endregion

    #region Handle Grass
    private bool CheckGrassSpawn()
    {
        int currentWeedCount = grassList.FindAll(g => g.activeInHierarchy).Count;
        if (currentWeedCount >= plantStats.MAX_WEED)
        {
            return false;
        }

        int multiplier = isWatered ? plantStats.WATER_MULTIPLIER : 1;
        if (UnityEngine.Random.value < plantStats.WEED_SPAWN_CHANCE * multiplier)
        {
            int tryCount = 0;
            
            do
            {
                int index = UnityEngine.Random.Range(0, grassList.Count);
                if (!grassList[index].activeInHierarchy)
                {
                    grassList[index].transform.localScale = Vector3.one * 0.1f; // Start small with scale 0.1
                    grassList[index].SetActive(true);
                    return true;
                }
                else
                {
                    tryCount++;
                }
            }
            while (tryCount < 100);
        }

        return false;
    }

    private void HandleGrassGrowth()
    {
        foreach (var grass in grassList)
        {
            if (grass.activeInHierarchy)
            {
                // If the plant is Watered, grow faster
                int mutiplier = isWatered ? plantStats.WATER_MULTIPLIER : 1;
                grass.transform.localScale += grass.transform.localScale * plantStats.WEED_GROWTH_SPEED * mutiplier * Time.deltaTime;

                if (grass.transform.localScale.x >= 1f)
                {
                    grass.transform.localScale = Vector3.one;
                }
            }
        }
    }

    public void CutWeed()
    {
        foreach (var grass in grassList)
        {
            if (grass.activeInHierarchy)
            {
                grass.SetActive(false);
                grass.transform.localScale = Vector3.zero;
            }
        }
    }
    #endregion

    #region Handle Watering
    private IEnumerator ProcessWaterVisual(int loadedWaterState, int waterState)
    {
        yield return new WaitUntil(() => DataPersistenceManager.Instance.isLoadedDataDone);

        Debug.Log("Done " + loadedWaterState + " " + waterState);

        // Remove existing watered visual
        switch (loadedWaterState)
        {
            case 0:
                RemoveWateredVisual(wateredSoilData);
                break;
            case 1:
                RemoveWateredVisual(wateredFadeOutSoilData);
                break;
            default:
                break;
        }

        if (isWatered)
        {
            // Load new watered visual
            switch (waterState)
            {
                case 0:
                    AddWateredVisual(wateredSoilData);
                    break;
                case 1:
                    AddWateredVisual(wateredFadeOutSoilData);
                    break;
                default:
                    break;
            }
        }
        else
        {
            waterTimer = 0;
            this.waterState = 0;
        }

        Debug.Log("Done Processed watered visual.");
    }

    public void StartWater()
    {
        waterTimer = plantStats.WATER_EXISTING_TIME / plantStats.TOTAL_WATER_LEVELS;
        waterState = 0;

        isWatered = true;
    }

    private void HandleWaterLevel()
    {
        waterTimer -= Time.deltaTime;

        if (waterTimer < 0)
        {
            waterState++;

            if (waterState >= plantStats.TOTAL_WATER_LEVELS)
            {
                isWatered = false;

                waterTimer = 0;
                waterState = 0;

                RemoveWateredVisual(wateredFadeOutSoilData);
            }
            else
            {
                RemoveWateredVisual(wateredSoilData);
                AddWateredVisual(wateredFadeOutSoilData);

                waterTimer = plantStats.WATER_EXISTING_TIME / plantStats.TOTAL_WATER_LEVELS;
            }
        }
    }

    private void RemoveWateredVisual(ObjectData objectData)
    {
        PlacementSystem.Instance.GridDataDictionary[objectData.gridType].RemoveObject(mainPosition);
        PlacementSystem.Instance.WateringState.ProcessDualGridVisual(PlacementSystem.Instance, objectData.gridType, objectData, mainPosition);
    }

    private void AddWateredVisual(ObjectData objectData)
    {
        PlacementSystem.Instance.AddObjectToGridData(objectData, objectData.gridType, mainPosition);
        PlacementSystem.Instance.WateringState.ProcessDualGridVisual(PlacementSystem.Instance, objectData.gridType, objectData, mainPosition);
    }

    public void ClearWateredSoil()
    {
        if (isWatered)
        {
            if (waterState == 0)
            {
                RemoveWateredVisual(wateredSoilData);
            }
            else if (waterState == 1)
            {
                RemoveWateredVisual(wateredFadeOutSoilData);
            }
        }
    }
    #endregion

    public void HarvestPlant()
    {

    }

    #region Save Load Plant Data
    public void LoadExistingData(PlantProgressData data)
    {
        mainPosition = data.mainPosition;

        var targetPosition = transform.position;
        targetPosition.y = data.yPosition;
        transform.position = targetPosition;

        long lastLoginTime = DataPersistenceManager.Instance.LastLoginTime;
        long secondsFromNow = (DateTime.Now.Ticks - lastLoginTime) / TimeSpan.TicksPerSecond;

        growthTime = data.currentGrowthTime + secondsFromNow;
        currentStateIndex = data.currentStateIndex;
        currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;

        while (growthTime >= currentStateTime && currentStateIndex < plantData.plantStates.Count - 1)
        {
            growthTime -= currentStateTime;
            currentStateIndex++;
            currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;
        }

        UpdatePlantStateVisual();

        if (currentStateIndex >= plantData.plantStates.Count - 1)
        {
            isDead = true;
            Debug.Log("Plant has died.");
        }

        waterTimer = data.waterTimer - secondsFromNow;
        waterState = data.waterState;
        isWatered = data.isWatered;

        int loadedWaterState = waterState;

        if (isWatered)
        {
            while (waterTimer < 0 && waterState < plantStats.TOTAL_WATER_LEVELS)
            {
                waterTimer += plantStats.WATER_EXISTING_TIME / plantStats.TOTAL_WATER_LEVELS;
                waterState++;
            }

            // Done watering
            if (waterState >= plantStats.TOTAL_WATER_LEVELS)
            {
                isWatered = false;
            }
            else
            {
                isWatered = true;
            }
        }

        Debug.Log($"IsWatered: {isWatered}; WaterTimer: {waterTimer}; WaterState: {waterState}");
        StartCoroutine(ProcessWaterVisual(loadedWaterState, waterState));
    }

    public PlantProgressData SavePlantData()
    {
        PlantProgressData data = new PlantProgressData
        {
            plantDataId = plantData.ID,
            mainPosition = mainPosition,
            currentStateIndex = currentStateIndex,
            currentGrowthTime = growthTime,
            yPosition = transform.position.y,
            waterTimer = waterTimer,
            waterState = waterState,
            isWatered = isWatered
        };

        return data;
    }
    #endregion
}
