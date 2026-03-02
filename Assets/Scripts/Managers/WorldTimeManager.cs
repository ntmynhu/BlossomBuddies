using UnityEngine;

public class WorldTimeManager : Singleton<WorldTimeManager>
{
    [SerializeField] private WorldTimeConfig timeConfig;

    public WorldTimeConfig WorldTimeConfig => timeConfig;

    public float IG_Hour_to_RT_Hour(float inGameHours)
    {
        return ((inGameHours * timeConfig.timeOfDay) / 24);
    }

    public float IG_Hour_to_RT_Second(float inGameHours)
    {
        return ((inGameHours * timeConfig.timeOfDay) / 24) * 3600;
    }
}
