using System;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset lightingPreset;
    [SerializeField, Range(0, 24)] private float timeOfDay;

    private float updateLightingInterval = 30f; // Update every 60 seconds
    private float updateLightingTimer = 0f;

    private DateTime currentTime;

    private void Start()
    {
        currentTime = DateTime.Now;
        timeOfDay = currentTime.Hour + currentTime.Minute / 60f + currentTime.Second / 3600f;
        UpdateLighting(timeOfDay / 24);

        updateLightingTimer = 0f; // Reset the timer after updating
    }

    private void Update()
    {
        //updateLightingTimer += Time.deltaTime;

        //if (updateLightingTimer >= updateLightingInterval)
        //{
        //    currentTime = DateTime.Now;
        //    timeOfDay = currentTime.Hour + currentTime.Minute / 60f + currentTime.Second / 3600f;
        //    UpdateLighting(timeOfDay / 24);

        //    Debug.Log(timeOfDay);
        //    updateLightingTimer = 0f; // Reset the timer after updating
        //}

        UpdateLighting(timeOfDay / 24);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = lightingPreset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = lightingPreset.FogColor.Evaluate(timePercent);

        directionalLight.color = lightingPreset.DirectionalColor.Evaluate(timePercent);
        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(timePercent * 360f - 90f, 170f, 0));
    }
}
