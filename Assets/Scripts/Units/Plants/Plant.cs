using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private PlantData plantData;
    [SerializeField] private List<GameObject> stateGameObjects;

    [SerializeField] private ObjectData wateredSoilData;
    [SerializeField] private ObjectData wateredFadeOutSoilData;

    private Vector3Int mainPosition;
    private float growthTime = 0;
    private int currentStateIndex = 0;
    private float currentStateTime = 0;

    private bool isDead = false;

    #region Handle Watering
    private float waterExistingTime = 20f;
    private float waterTimer;
    private int waterState;
    private int totalWaterLevels = 2;
    private bool isWatered = false;
    #endregion

    #region Properties
    public Vector3Int MainPosition { get => mainPosition; set => mainPosition = value; }
    public ObjectData WateredSoilData => wateredSoilData;
    public ObjectData WateredFadeOutSoilData => wateredFadeOutSoilData;
    public bool IsDead => isDead;
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

        if (isDead)
        {
            return;
        }

        growthTime += Time.deltaTime;

        if (growthTime >= currentStateTime)
        {
            AdvanceToNextState();
        }
    }

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
            while (waterTimer < 0 && waterState < totalWaterLevels)
            {
                waterTimer += waterExistingTime / totalWaterLevels;
                waterState++;
            }

            // Done watering
            if (waterState >= totalWaterLevels)
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
        waterTimer = waterExistingTime / totalWaterLevels;
        waterState = 0;

        isWatered = true;
    }

    private void HandleWaterLevel()
    {
        waterTimer -= Time.deltaTime;

        if (waterTimer < 0)
        {
            waterState++;

            if (waterState >= totalWaterLevels)
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

                waterTimer = waterExistingTime / totalWaterLevels;
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
}
