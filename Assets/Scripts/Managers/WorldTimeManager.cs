using UnityEngine;

public class WorldTimeManager : Singleton<WorldTimeManager>
{
    [SerializeField] private WorldTimeConfig timeConfig;

    public WorldTimeConfig WorldTimeConfig => timeConfig;

    public float IG_to_RT_Hour(float inGameHours)
    {
        return ((inGameHours * timeConfig.timeOfDay) / 24);
    }
}
