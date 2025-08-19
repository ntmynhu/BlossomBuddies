using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private PlantData plantData;
    [SerializeField] private List<GameObject> stateGameObjects;

    private Vector3Int mainPosition;
    private float growthTime = 0;
    private int currentStateIndex = 0;
    private float currentStateTime = 0;

    private bool isDead = false;

    public Vector3Int MainPosition { get => mainPosition; set => mainPosition = value; }
    public PlantData PlantData => plantData;
    public int CurrentStateIndex => currentStateIndex;
    public float CurrentGrowthTime => growthTime;

    private void Start()
    {
        currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;
    }

    private void Update()
    {
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

        growthTime = data.currentGrowthTime;
        currentStateIndex = data.currentStateIndex;
        currentStateTime = plantData.plantStates[currentStateIndex].time * 3600;

        UpdatePlantStateVisual();

        if (currentStateIndex >= plantData.plantStates.Count - 1)
        {
            isDead = true;
            Debug.Log("Plant has died.");
        }
    }
}
