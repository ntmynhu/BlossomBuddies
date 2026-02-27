using UnityEngine;

public class WorldTimeManager : Singleton<WorldTimeManager>
{
    [SerializeField] private WorldTimeConfig timeConfig;

    public WorldTimeConfig WorldTimeConfig => timeConfig;
}
