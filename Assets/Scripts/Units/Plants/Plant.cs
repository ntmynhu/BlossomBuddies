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
        currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;
    }

    private void Update()
    {
        if (isWatered)
        {
            HandleWaterLevel();
        }

        // Handle Grass Spawn
        tickTimer -= Time.deltaTime;
        if (tickTimer < 0)
        {
            tickTimer = plantStats.WEED_TICK_TIME;
            CheckGrassSpawn();
        }

        HandleGrassGrowth(Time.deltaTime);
        //

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
        growthTime -= CalculateWeedPenalty(Time.deltaTime);

        if (growthTime >= currentStateTime)
        {
            AdvanceToNextState();
        }
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

    private void HandleGrassGrowth(float time)
    {
        foreach (var grass in grassList)
        {
            if (grass.activeInHierarchy)
            {
                // If the plant is Watered, grow faster
                int mutiplier = isWatered ? plantStats.WATER_MULTIPLIER : 1;
                grass.transform.localScale += grass.transform.localScale * plantStats.WEED_GROWTH_SPEED * mutiplier * time;

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

                GameManager.Instance.AddHeart(1);
            }
        }
    }

    private float CalculateWeedPenalty(float time)
    {
        float totalPenalty = 0;
        int multiplier = isWatered ? plantStats.WATER_MULTIPLIER : 1;
        foreach (var grass in grassList)
        {
            if (grass.activeInHierarchy)
            {
                totalPenalty += plantStats.WEED_PENALTY_SPEED * time * grass.transform.localScale.x * multiplier;
            }
        }

        return totalPenalty;
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

        GameManager.Instance.AddHeart(1);
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

        // Load Grass Displayed
        if (data.grassDataList != null && data.grassDataList.Count == grassList.Count)
        {
            for (int i = 0; i < grassList.Count; i++)
            {
                grassList[i].SetActive(data.grassDataList[i].isActive);
                grassList[i].transform.localScale = data.grassDataList[i].localScale;
            }
        }
        else
        {
            Debug.LogWarning("Grass data list is null or does not match the number of grass objects.");
        }

        // Load Plant State
        growthTime = data.currentGrowthTime;
        currentStateIndex = data.currentStateIndex;
        currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;
        waterTimer = data.waterTimer;
        waterState = data.waterState;
        tickTimer = data.tickTimer;
        isWatered = data.isWatered;

        float totalSeconds = secondsFromNow;

        // While totalSeconds Greater than 0
        while (totalSeconds > 0)
        {
            float timeToProcess = Mathf.Min(totalSeconds, plantStats.WEED_TICK_TIME);

            growthTime += timeToProcess;

            // Calculate Growth Time for 1 Tick with water
            if (isWatered)
            {
                // If the waterTimer is less than WEED_TICK_TIME, means it will run out of water (1 state) in this tick
                if (waterTimer < timeToProcess)
                {
                    // Calculate Growth for the remaining water time
                    growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * waterTimer;
                    growthTime -= CalculateWeedPenalty(waterTimer);
                    HandleGrassGrowth(waterTimer);

                    // Remove 1 water state
                    waterState++;
                    float remainingTickTime = timeToProcess - waterTimer;

                    if (waterState < plantStats.TOTAL_WATER_LEVELS)
                    {
                        // If still watered, apply growth for the remaining tick time
                        waterTimer = plantStats.WATER_EXISTING_TIME / plantStats.TOTAL_WATER_LEVELS - remainingTickTime;

                        // Calculate Growth for the remaining tick time if still watered
                        growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * remainingTickTime;
                        growthTime -= CalculateWeedPenalty(remainingTickTime);
                        HandleGrassGrowth(remainingTickTime);
                    }
                    else
                    {
                        // If not watered anymore, Calculate growth for the remaining tick time without water bonus
                        isWatered = false;
                        waterTimer = 0;
                        growthTime -= CalculateWeedPenalty(remainingTickTime);
                        HandleGrassGrowth(remainingTickTime);
                    }
                }
                else
                {
                    // Still has water for this tick
                    growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * timeToProcess;
                    growthTime -= CalculateWeedPenalty(timeToProcess);
                    HandleGrassGrowth(timeToProcess);
                }
            }
            else
            {
                growthTime -= CalculateWeedPenalty(timeToProcess);
                HandleGrassGrowth(timeToProcess);
            }

            // Check Grass Spawn
            tickTimer -= timeToProcess;
            if (tickTimer < 0)
            {
                CheckGrassSpawn();
                tickTimer += plantStats.WEED_TICK_TIME; // Add the negative tickTimer to reset
            }
            
            while (growthTime >= currentStateTime && currentStateIndex < plantData.plantStates.Count - 1)
            {
                growthTime -= currentStateTime;
                currentStateIndex++;
                currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;
            }

            if (currentStateIndex >= plantData.plantStates.Count - 1 || growthTime < 0)
            {
                isDead = true;
                Debug.Log("Plant has died.");
            }

            totalSeconds -= timeToProcess;
        }

        UpdatePlantStateVisual();

        // Load Watered Soil Displayed
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
        List<GrassData> grassDataList = new List<GrassData>();
        foreach (var grass in grassList)
        {
            GrassData grassData = new GrassData
            {
                isActive = grass.activeInHierarchy,
                localScale = grass.transform.localScale
            };
            grassDataList.Add(grassData);
        }

        PlantProgressData data = new PlantProgressData
        {
            plantDataId = plantData.ID,
            mainPosition = mainPosition,
            currentStateIndex = currentStateIndex,
            currentGrowthTime = growthTime,
            yPosition = transform.position.y,
            waterTimer = waterTimer,
            waterState = waterState,
            isWatered = isWatered,
            grassDataList = grassDataList,
            tickTimer = tickTimer
        };

        return data;
    }
    #endregion
}
