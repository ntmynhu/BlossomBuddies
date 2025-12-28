using System;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset lightingPreset;

    [Header("Time")]
    [SerializeField, Range(0, 24)] private float timeOfDay;

    [Header("Smoothing")]
    [Tooltip("How quickly the lighting catches up to the target time (higher = faster).")]
    [SerializeField] private float timeSmoothSpeed = 2.5f;

    [Tooltip("Optional: smooth light rotation separately.")]
    [SerializeField] private float rotationSmoothSpeed = 6f;

    private float updateLightingTimer = 0f;

    private DateTime currentTime;
    private WorldTimeConfig timeConfig;

    private float targetTimePercent;
    private float currentTimePercent;

    private void Start()
    {
        timeConfig = WorldTimeManager.Instance.WorldTimeConfig;

        // Init target from real time
        currentTime = DateTime.Now;
        timeOfDay = currentTime.Hour + currentTime.Minute / 60f + currentTime.Second / 3600f;
        targetTimePercent = Mathf.Repeat(timeOfDay / timeConfig.timeOfDay, 1f);

        currentTimePercent = targetTimePercent;

        ApplyLighting(currentTimePercent, instantRotation: true);
        updateLightingTimer = 0f;
    }

    private void Update()
    {
        updateLightingTimer += Time.deltaTime;
        if (updateLightingTimer >= timeConfig.updateLigtingTime)
        {
            currentTime = DateTime.Now;
            timeOfDay = currentTime.Hour + currentTime.Minute / 60f + currentTime.Second / 3600f;
            targetTimePercent = Mathf.Repeat(timeOfDay / timeConfig.timeOfDay, 1f);

            updateLightingTimer = 0f;
        }

        currentTimePercent = MoveTowardWrapped01(currentTimePercent, targetTimePercent, timeSmoothSpeed, Time.deltaTime);

        ApplyLighting(currentTimePercent, instantRotation: false);
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
}
