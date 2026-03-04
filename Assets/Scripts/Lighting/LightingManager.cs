using System;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : Singleton<LightingManager>
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset lightingPreset;
    [SerializeField] private List<TimeSetting> timeSettings;
    [SerializeField] private List<TimeLightValue> timeLightValues;

    [Header("Smoothing")]
    [Tooltip("How quickly the lighting catches up to the target time (higher = faster).")]
    [SerializeField] private float timeSmoothSpeed = 2.5f;

    [Tooltip("Optional: smooth light rotation separately.")]
    [SerializeField] private float rotationSmoothSpeed = 6f;

    private float updateLightingTimer = 0f;

    private float currentTimePercentage;
    private float targetTimePercentage;

    private WorldTimeConfig timeConfig;
    private TimeOfDay currentTimeOfDay = TimeOfDay.None;

    private float currentLightValue = 0f;

    public float CurrentLightValue => currentLightValue;
    public TimeOfDay CurrentTimeOfDay => currentTimeOfDay;

    private void Start()
    {
        timeConfig = WorldTimeManager.Instance.WorldTimeConfig;

        currentTimePercentage = WorldTimeManager.Instance.CalculateInGameTimeFromRealTime(DateTime.Now) / WorldTimeManager.Instance.WorldTimeConfig.hoursInDay;
        targetTimePercentage = Mathf.Repeat(currentTimePercentage, 1f);

        ApplyLighting(currentTimePercentage, instantRotation: true);
        updateLightingTimer = 0f;
    }

    private void Update()
    {
        updateLightingTimer += Time.deltaTime;

        if (updateLightingTimer >= timeConfig.updateLigtingTime)
        {
            float addTimePercent = (((timeConfig.updateLigtingTime / 3600f) * timeConfig.hoursInDay) / timeConfig.timeOfDay) / timeConfig.hoursInDay;

            Debug.Log(addTimePercent);

            currentTimePercentage += addTimePercent;
            targetTimePercentage = Mathf.Repeat(currentTimePercentage, 1f);

            updateLightingTimer = 0f;
        }

        currentTimePercentage = MoveTowardWrapped01(currentTimePercentage, targetTimePercentage, timeSmoothSpeed, Time.deltaTime);
        ApplyLighting(currentTimePercentage, instantRotation: false);
    }

    private void ApplyLighting(float timePercent, bool instantRotation)
    {
        RenderSettings.ambientLight = lightingPreset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = lightingPreset.FogColor.Evaluate(timePercent);

        directionalLight.color = lightingPreset.DirectionalColor.Evaluate(timePercent);

        // Light rotation
        Quaternion targetRot = Quaternion.Euler(timePercent * 360f - 90f, 170f, 0f);

        if (instantRotation)
        {
            directionalLight.transform.localRotation = targetRot;
        }
        else
        {
            directionalLight.transform.localRotation =
                Quaternion.Slerp(directionalLight.transform.localRotation, targetRot, 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime));
        }

        var nextTimeOfDay = GetTimeOfDay(timePercent);
        if (nextTimeOfDay != currentTimeOfDay)
        {
            currentTimeOfDay = nextTimeOfDay;

            switch (currentTimeOfDay)
            {
                case TimeOfDay.Morning:
                    Debug.Log("Good Morning!");
                    currentLightValue = timeLightValues.Find(x => x.timeOfDay == TimeOfDay.Morning).lightValue;
                    break;
                case TimeOfDay.Afternoon:
                    Debug.Log("Good Afternoon!");
                    currentLightValue = timeLightValues.Find(x => x.timeOfDay == TimeOfDay.Afternoon).lightValue;
                    break;
                case TimeOfDay.Evening:
                    Debug.Log("Good Evening!");
                    currentLightValue = timeLightValues.Find(x => x.timeOfDay == TimeOfDay.Evening).lightValue;
                    break;
                case TimeOfDay.Night:
                    Debug.Log("Good Night!");
                    currentLightValue = timeLightValues.Find(x => x.timeOfDay == TimeOfDay.Night).lightValue;
                    break;
            }

            Debug.Log("Time of Day changed to: " + currentTimeOfDay);
            GameEventManager.Instance.OnTimeOfDayChanged(currentTimeOfDay);
        }
    }

    // Smoothly approaches target on a circular [0..1) range, choosing the shortest path (important at midnight).
    private static float MoveTowardWrapped01(float current, float target, float speed, float dt)
    {
        // shortest signed distance in [-0.5, 0.5)
        float delta = Mathf.DeltaAngle(current * 360f, target * 360f) / 360f;

        // exponential smoothing (frame-rate independent)
        float t = 1f - Mathf.Exp(-speed * dt);
        return Mathf.Repeat(current + delta * t, 1f);
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
public class TimeLightValue
{
    public TimeOfDay timeOfDay;
    public float lightValue;
}

[Serializable]
public class TimeWaterValue
{
    public TimeOfDay timeOfDay;
    public float waterValue;
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
