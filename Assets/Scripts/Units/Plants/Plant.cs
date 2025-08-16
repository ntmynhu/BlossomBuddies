using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private PlantData plantData;
    [SerializeField] private List<GameObject> stateGameObjects;

    private float growthTime = 0;
    private int currentStateIndex = 0;

    private float currentStateTime = 0;

    private bool isDead = false;

    private void Start()
    {
        currentStateTime = plantData.plantStates[currentStateIndex].time;
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

        currentStateTime = plantData.plantStates[currentStateIndex].time;
        UpdatePlantStateVisual();

        if (currentStateIndex >= plantData.plantStates.Count - 1)
        {
            isDead = true;
            Debug.Log("Plant has died.");
            return;
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

    public PlantData GetPlantData()
    {
        return plantData;
    }
}
