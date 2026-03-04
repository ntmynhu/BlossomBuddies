using System;
using UnityEngine;

public class WorldTimeManager : Singleton<WorldTimeManager>
{
    [SerializeField] private WorldTimeConfig timeConfig;
    public WorldTimeConfig WorldTimeConfig => timeConfig;

    private float baseRealTimeHour = 0; // To calculate the IN GAME time hour based on REAL TIME (e.g. 0 meaning IN GAME time starts at 0:00)

    private void Start()
    {
        Debug.Log("Test realtime");
        CalculateInGameTimeFromRealTime(DateTime.Now);
    }

    public float IG_Hour_to_RT_Hour(float inGameHours)
    {
        return ((inGameHours * timeConfig.timeOfDay) / 24);
    }

    public float IG_Hour_to_RT_Second(float inGameHours)
    {
        return ((inGameHours * timeConfig.timeOfDay) / 24) * 3600;
    }

    public float CalculateInGameTimeFromRealTime(DateTime realTime)
    {
        Debug.Log($"Real Time: {realTime.Hour}:{realTime.Minute}:{realTime.Second}");

        float realTimeHour = realTime.Hour + (float)realTime.Minute / 60 + (float)realTime.Second / 3600;
        Debug.Log($"Real Time Hour: {realTimeHour}");

        float calculateRealtime = realTimeHour - baseRealTimeHour;
        Debug.Log($"Calculate Real Time: {calculateRealtime}");

        float inGameTotalHour = ((calculateRealtime / timeConfig.timeOfDay) - (int)(calculateRealtime / timeConfig.timeOfDay)) * timeConfig.hoursInDay;
        Debug.Log($"In Game Total Hour: {inGameTotalHour}");

        int inGameHour = (int)inGameTotalHour;
        int inGameMinute = inGameTotalHour - inGameHour > 0 ? (int)((inGameTotalHour - inGameHour) * 60) : 0;
        int inGameSecond = inGameTotalHour - inGameHour - (inGameMinute / 60f) > 0 ? (int)(((inGameTotalHour - inGameHour) - (inGameMinute / 60f)) * 3600) : 0;

        Debug.Log($"In Game Time: {inGameHour}:{inGameMinute}:{inGameSecond}");

        return inGameTotalHour;
    }
}
