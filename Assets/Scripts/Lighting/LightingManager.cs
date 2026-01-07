using System;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset lightingPreset;
    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField] private List<TimeSetting> timeSettings;

    private float updateLightingInterval = 30f; // Update every 60 seconds
    private float updateLightingTimer = 0f;

    private DateTime currentTime;
    private TimeOfDay currentTimeOfDay = TimeOfDay.None;

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

        var nextTimeOfDay = GetTimeOfDay(timePercent);
        if (nextTimeOfDay != currentTimeOfDay)
        {
            currentTimeOfDay = nextTimeOfDay;

            switch (currentTimeOfDay)
            {
                case TimeOfDay.Morning:
                    Debug.Log("Good Morning!");
                    break;
                case TimeOfDay.Afternoon:
                    Debug.Log("Good Afternoon!");
                    break;
                case TimeOfDay.Evening:
                    Debug.Log("Good Evening!");
                    break;
                case TimeOfDay.Night:
                    Debug.Log("Good Night!");
                    break;
            }

            Debug.Log("Time of Day changed to: " + currentTimeOfDay);
            GameEventManager.Instance.OnTimeOfDayChanged(currentTimeOfDay);
        }
    }

    private TimeOfDay GetTimeOfDay(float timePercent)
    {
        for (int i = 0; i < timeSettings.Count; i++)
        {
            TimeSetting currentSetting = timeSettings[i];
            TimeSetting nextSetting = timeSettings[(i + 1) % timeSettings.Count];

            if (timePercent >= currentSetting.timePercent && timePercent < nextSetting.timePercent)
            {
                return currentSetting.timeOfDay;
            }
        }

        return timeSettings[0].timeOfDay; // Default to the first time of day if not found
    }
}

[Serializable]
public class TimeSetting
{
    public TimeOfDay timeOfDay;
    public float timePercent;
}

[Serializable]
public enum TimeOfDay
{
    None,
    Morning,
    Afternoon,
    Evening,
    Night
}
