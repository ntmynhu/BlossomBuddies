using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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
    private float deadTime = 0;

    private int currentStateIndex = 0;
    private float currentStateGrowthTime = 0;
    private float currentStateDeadTime = 0;

    private bool isDead = false;
    private bool isFullyGrown = false;

    private Dictionary<PlantMainStatsType, float> currentMainStats = new Dictionary<PlantMainStatsType, float>();
    private float mainTickTimer = 0;

    #region Watering Variables
    private float waterTimer;
    private int waterState;
    private bool isWatered = false;
    #endregion

    #region Grass Variables
    private float weedTickTimer = 0;
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
        // Calculate the current state time 
        currentStateGrowthTime = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantData.plantStates[currentStateIndex].growthTime);
        currentStateDeadTime = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantData.plantStates[currentStateIndex].deadTime);
    }

    private void Update()
    {
        if (isWatered)
        {
            HandleWaterLevel();
        }

        // Handle Grass Spawn
        weedTickTimer -= Time.deltaTime;
        if (weedTickTimer < 0)
        {
            weedTickTimer = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.MAIN_STAT_TICK);
            CheckGrassSpawn();
        }
        //

        HandleMainStats();
        HandleGrassGrowth(Time.deltaTime);

        if (isDead)
        {
            return;
        }

        // Handle Plant Growth
        CalculateGrowthTime();

        if (growthTime >= currentStateGrowthTime)
        {
            AdvanceToNextState();
        }
        //
    }

    public void InitDefautlMainStats()
    {
        currentMainStats = new Dictionary<PlantMainStatsType, float>();
        foreach (PlantMainStatsType mainStat in PlantMainStatsType.GetValues(typeof(PlantMainStatsType)))
        {
            if (plantData.defaultMainStats.Exists(s => s.type == mainStat))
            {
                currentMainStats[mainStat] = plantData.defaultMainStats.Find(s => s.type == mainStat).value;
            }
            else
            {
                currentMainStats[mainStat] = 0;
            }
        }
    }

    private void HandleMainStats()
    {
        mainTickTimer -= Time.deltaTime;
        
        if (mainTickTimer < 0)
        {
            // Handle Light stat
            currentMainStats[PlantMainStatsType.Light] += LightingManager.Instance.CurrentLightValue;
            currentMainStats[PlantMainStatsType.Light] = Mathf.Clamp(currentMainStats[PlantMainStatsType.Light], plantStats.MIN_MAIN_STAT_VALUE, plantStats.MAX_MAIN_STAT_VALUE);

            // Handle Water stat
            if (isWatered)
            {
                if (waterState == 0)
                {
                    currentMainStats[PlantMainStatsType.Water] += plantStats.DARK_WATER_VALUE;
                }
                else if (waterState == 1)
                {
                    currentMainStats[PlantMainStatsType.Water] += plantStats.LIGHT_WATER_VALUE;
                }
            }
            else
            {
                float waterDecreaseValue = plantStats.TIME_WATER_DECREASE_VALUES.Find(v => v.timeOfDay == LightingManager.Instance.CurrentTimeOfDay).waterValue;
                currentMainStats[PlantMainStatsType.Water] += waterDecreaseValue;
            }

            currentMainStats[PlantMainStatsType.Water] = Mathf.Clamp(currentMainStats[PlantMainStatsType.Water], plantStats.MIN_MAIN_STAT_VALUE, plantStats.MAX_MAIN_STAT_VALUE);

            // Handle Nutrient stat
            currentMainStats[PlantMainStatsType.Nutrient] += plantStats.FERTILIZE_DECREASE_VALUE;
            currentMainStats[PlantMainStatsType.Nutrient] = Mathf.Clamp(currentMainStats[PlantMainStatsType.Nutrient], plantStats.MIN_MAIN_STAT_VALUE, plantStats.MAX_MAIN_STAT_VALUE);

            DebugMainStats();

            mainTickTimer = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.MAIN_STAT_TICK);
        }
    }

    private void DebugMainStats()
    {
        foreach (var kvp in currentMainStats)
        {
            Debug.Log($"Main Stat: {kvp.Key}, Value: {kvp.Value}");
        }
    }

    #region Handle Flower State
    private void CalculateGrowthTime()
    {
        bool isGrowing = CheckGrowCondition();

        if (isGrowing)
        {
            growthTime += Time.deltaTime;
            deadTime = 0;

            //// Apply Water Bonus
            //if (isWatered)
            //{
            //    growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * Time.deltaTime;
            //}

            // Apply Weed Penalty
            growthTime -= CalculateWeedPenalty(Time.deltaTime);
        }
        else
        {
            deadTime += Time.deltaTime;

            if (deadTime >= currentStateDeadTime)
            {
                UpdateDeadState();
            }
        }
    }

    private bool CheckGrowCondition()
    {
        bool isGrowing = false;

        foreach (var condition in plantData.plantStates[currentStateIndex].conditions)
        {
            float mainStatValue = currentMainStats[condition.type];
            float percentage = mainStatValue / plantStats.MAX_MAIN_STAT_VALUE;

            Color gradientColor = condition.conditionRange.Evaluate(percentage);

            if (gradientColor == Color.green)
            {
                // Valid condition, should increase growth time
                isGrowing = true;
            }
            else if (gradientColor == Color.blue)
            {
                // Optimal condition, should increase growth time increase scale effect
                isGrowing = true;
            }
            else if (gradientColor == Color.red)
            {
                // Dead zone
                isGrowing = false;
                break;
            }
        }

        return isGrowing;
    }

    private void AdvanceToNextState()
    {
        if (currentStateIndex == plantData.plantStates.Count - 2)
        {
            isFullyGrown = true;
            growthTime = currentStateGrowthTime; // Cap the growth time at the max for fully grown state
            return;
        }

        growthTime = 0;
        currentStateIndex++;

        currentStateGrowthTime = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantData.plantStates[currentStateIndex].growthTime);
        currentStateDeadTime = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantData.plantStates[currentStateIndex].deadTime);
        UpdatePlantStateVisual();
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

    private void UpdateDeadState()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log("Plant has died.");

        currentStateIndex = plantData.plantStates.Count - 1; // Set to dead state index
        UpdatePlantStateVisual();
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
        waterTimer = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.WATER_EXISTING_TIME) / plantStats.TOTAL_WATER_LEVELS;
        waterState = 0;

        isWatered = true;

        GameManager.Instance.AddHeart(1);
    }

    public void StartFertilizer()
    {
        currentMainStats[PlantMainStatsType.Nutrient] += plantStats.FERTILIZE_ADDED_VALUE;
        currentMainStats[PlantMainStatsType.Nutrient] = Mathf.Clamp(currentMainStats[PlantMainStatsType.Nutrient], plantStats.MIN_MAIN_STAT_VALUE, plantStats.MAX_MAIN_STAT_VALUE);

        DebugMainStats();
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

                waterTimer = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.WATER_EXISTING_TIME) / plantStats.TOTAL_WATER_LEVELS;
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
    private void CalculateMainStatsAtTime(float timeToProcess, float currentTimePercent, out float lightValue, out float waterValue, out float nutrientValue)
    {
        // Process Light Stat
        lightValue = LightingManager.Instance.GetLightValue(currentTimePercent);

        // Process Water Stat
        waterValue = 0;
        if (isWatered)
        {
            if (waterTimer > timeToProcess)
            {
                if (waterState == 0)
                {
                    waterValue = plantStats.DARK_WATER_VALUE;
                }
                else if (waterState == 1)
                {
                    waterValue = plantStats.LIGHT_WATER_VALUE;
                }
            }
            else
            {
                if (waterState == 0)
                {
                    waterValue = plantStats.LIGHT_WATER_VALUE;
                }
                else if (waterState == 1)
                {
                    waterValue = plantStats.TIME_WATER_DECREASE_VALUES.Find(v => v.timeOfDay == LightingManager.Instance.GetTimeOfDay(currentTimePercent)).waterValue;
                }
            }
        }
        else
        {
            waterValue = plantStats.TIME_WATER_DECREASE_VALUES.Find(v => v.timeOfDay == LightingManager.Instance.GetTimeOfDay(currentTimePercent)).waterValue;
        }

        // Process Nutrient Stat
        nutrientValue = plantStats.FERTILIZE_DECREASE_VALUE;
    }

    private void CalculateGrowthTimeFromLoadedMainStat(float timeToProcess)
    {
        bool isGrowing = CheckGrowCondition();

        if (isGrowing)
        {
            growthTime += timeToProcess;
            deadTime = 0;
        }
        else
        {
            deadTime += timeToProcess;

            if (deadTime >= currentStateDeadTime)
            {
                UpdateDeadState();
            }
        }
    }

    public void LoadExistingData(PlantProgressData data)
    {
        mainPosition = data.mainPosition;

        var targetPosition = transform.position;
        targetPosition.y = data.yPosition;
        transform.position = targetPosition;

        long lastLoginTime = DataPersistenceManager.Instance.LastLoginTime;
        long secondsFromNow = (DateTime.Now.Ticks - lastLoginTime) / TimeSpan.TicksPerSecond;

        // Load Main stats
        currentMainStats = new Dictionary<PlantMainStatsType, float>();
        foreach (PlantMainStatsType mainStat in PlantMainStatsType.GetValues(typeof(PlantMainStatsType)))
        {
            if (data.plantMainStatList != null && data.plantMainStatList.Exists(s => s.type == mainStat))
            {
                currentMainStats[mainStat] = data.plantMainStatList.Find(s => s.type == mainStat).value;
            }
            else
            {
                if (plantData.defaultMainStats.Exists(s => s.type == mainStat))
                {
                    currentMainStats[mainStat] = plantData.defaultMainStats.Find(s => s.type == mainStat).value;
                }
                else
                {
                    currentMainStats[mainStat] = 0;
                }
            }
        }

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
        deadTime = data.currentDeadTime;
        currentStateIndex = data.currentStateIndex;
        waterTimer = data.waterTimer;
        waterState = data.waterState;
        isWatered = data.isWatered;
        mainTickTimer = data.mainTickTimer;

        currentStateGrowthTime = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantData.plantStates[currentStateIndex].growthTime);
        currentStateDeadTime = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantData.plantStates[currentStateIndex].deadTime);

        DateTime lastLoginDateTime = new DateTime(lastLoginTime);
        float inGameHoursLoaded = WorldTimeManager.Instance.CalculateInGameTimeFromRealTime(lastLoginDateTime);
        float timePercentLoaded = inGameHoursLoaded / WorldTimeManager.Instance.WorldTimeConfig.hoursInDay;

        float totalSeconds = secondsFromNow;

        // While totalSeconds Greater than 0
        while (totalSeconds > 0)
        {
            float timeToProcess = Mathf.Min(totalSeconds, WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.MAIN_STAT_TICK));

            mainTickTimer -= timeToProcess;
            if (mainTickTimer > 0) // If still has time for main stat tick, means we can process without any change in main stat
            {
                Debug.Log($"Processing {timeToProcess} seconds with existing main stat values. Main tick timer: {mainTickTimer}");
                CalculateGrowthTimeFromLoadedMainStat(timeToProcess);
            }
            else
            {
                // If main change happens during the loaded time => calculate the main state base on the time it changes
                float afterMainTickTime = -mainTickTimer;
                float beforeMainTickTime = timeToProcess - afterMainTickTime;

                // Calculate growth for the time before main stat tick
                Debug.Log($"Processing {beforeMainTickTime} seconds before main stat tick with existing main stat values. Main tick timer: 0");
                CalculateGrowthTimeFromLoadedMainStat(beforeMainTickTime);

                // Calculate the main stat change at the tick
                timePercentLoaded += WorldTimeManager.Instance.RT_Second_to_IG_Hour(afterMainTickTime) / WorldTimeManager.Instance.WorldTimeConfig.hoursInDay;
                timePercentLoaded = Mathf.Repeat(timePercentLoaded, 1f); // Ensure it stays within [0,1]

                float lightValue, waterValue, nutrientValue;
                CalculateMainStatsAtTime(beforeMainTickTime, timePercentLoaded, out lightValue, out waterValue, out nutrientValue);

                currentMainStats[PlantMainStatsType.Light] += lightValue;
                currentMainStats[PlantMainStatsType.Water] += waterValue;
                currentMainStats[PlantMainStatsType.Nutrient] += nutrientValue;

                // Calculate growth for the time after main stat tick with the new main stat values
                Debug.Log($"Processing {afterMainTickTime} seconds after main stat tick with new main stat values. Main tick timer: {mainTickTimer}");
                CalculateGrowthTimeFromLoadedMainStat(afterMainTickTime);

                // Add time to mainTickTimer for the next tick
                mainTickTimer += WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.MAIN_STAT_TICK);
            }

            // Calculate main stats - LIGHT
            timePercentLoaded += WorldTimeManager.Instance.RT_Second_to_IG_Hour(timeToProcess) / WorldTimeManager.Instance.WorldTimeConfig.hoursInDay; // Convert seconds to hours
            float processedLightValue = LightingManager.Instance.GetLightValue(timePercentLoaded);

            currentMainStats[PlantMainStatsType.Light] += processedLightValue;

            //growthTime += timeToProcess;

            // Calculate Growth Time for 1 Tick with water
            if (isWatered)
            {
                // If the waterTimer is less than WEED_TICK_TIME, means it will run out of water (1 state) in this tick
                if (waterTimer < timeToProcess)
                {
                    //// Calculate Growth for the remaining water time
                    //growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * waterTimer;
                    if (CheckGrowCondition()) growthTime -= CalculateWeedPenalty(waterTimer);
                    HandleGrassGrowth(waterTimer);

                    // Remove 1 water state
                    waterState++;
                    float remainingTickTime = timeToProcess - waterTimer;

                    if (waterState < plantStats.TOTAL_WATER_LEVELS)
                    {
                        // If still watered, apply growth for the remaining tick time
                        waterTimer = WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.WATER_EXISTING_TIME) / plantStats.TOTAL_WATER_LEVELS - remainingTickTime;

                        //// Calculate Growth for the remaining tick time if still watered
                        //growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * remainingTickTime;
                        if (CheckGrowCondition()) growthTime -= CalculateWeedPenalty(remainingTickTime);
                        HandleGrassGrowth(remainingTickTime);
                    }
                    else
                    {
                        // If not watered anymore, Calculate growth for the remaining tick time without water bonus
                        isWatered = false;
                        waterTimer = 0;
                        if (CheckGrowCondition()) growthTime -= CalculateWeedPenalty(remainingTickTime);
                        HandleGrassGrowth(remainingTickTime);
                    }
                }
                else
                {
                    // Still has water for this tick
                    //growthTime += plantStats.WATER_BONUS_GROWTH_SPEED * (plantStats.TOTAL_WATER_LEVELS - waterState) * timeToProcess;
                    if (CheckGrowCondition()) growthTime -= CalculateWeedPenalty(timeToProcess);
                    HandleGrassGrowth(timeToProcess);
                }
            }
            else
            {
                if (CheckGrowCondition()) growthTime -= CalculateWeedPenalty(timeToProcess);
                HandleGrassGrowth(timeToProcess);
            }

            // Check Grass Spawn
            weedTickTimer -= timeToProcess;
            if (weedTickTimer < 0)
            {
                CheckGrassSpawn();
                weedTickTimer += WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.MAIN_STAT_TICK); // Add the negative tickTimer to reset
            }
            
            while (growthTime >= currentStateGrowthTime && currentStateIndex < plantData.plantStates.Count - 1)
            {
                AdvanceToNextState();
            }

            if (currentStateIndex >= plantData.plantStates.Count - 1 || deadTime >= currentStateDeadTime)
            {
                UpdateDeadState();
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
                waterTimer += WorldTimeManager.Instance.IG_Hour_to_RT_Second(plantStats.WATER_EXISTING_TIME) / plantStats.TOTAL_WATER_LEVELS;
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

        List<PlantMainStat> plantMainStatsList = new List<PlantMainStat>();
        foreach (var kvp in currentMainStats)
        {
            PlantMainStat mainStat = new PlantMainStat
            {
                type = kvp.Key,
                value = kvp.Value
            };
            plantMainStatsList.Add(mainStat);
        }

        PlantProgressData data = new PlantProgressData
        {
            plantDataId = plantData.ID,
            mainPosition = mainPosition,
            currentStateIndex = currentStateIndex,
            currentGrowthTime = growthTime,
            currentDeadTime = deadTime,
            yPosition = transform.position.y,
            waterTimer = waterTimer,
            waterState = waterState,
            isWatered = isWatered,
            grassDataList = grassDataList,
            weedTickTimer = weedTickTimer,
            mainTickTimer = mainTickTimer,
            plantMainStatList = plantMainStatsList
        };

        return data;
    }
    #endregion
}
