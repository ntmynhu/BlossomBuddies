using UnityEngine;

[CreateAssetMenu(fileName = "PlantStats", menuName = "Scriptable Objects/PlantStats")]
public class PlantStats : ScriptableObject
{
    [Header("Weed Stats")]
    [SerializeField] private float weedTickTime; // Checking every hour
    [SerializeField] private float weedSpawnChance; // 0.5% chance being Spawn every tick
    [SerializeField] private float weedGrowthSpeed; // Grow 0.003f scale every second
    [SerializeField] private float weedPenaltySpeed; // Decrease 50% growth speed per weed
    [SerializeField] private int waterMultiplier; // Weed Stats are increased by 3 times when watered
    [SerializeField] private int maxWeed;

    [Header("Water Stats")]
    [SerializeField] private float waterExistingTime;
    [SerializeField] private float waterBonusGrowthSpeed; // xn growth speed
    [SerializeField] private int totalWaterLevels;

    #region Properties
    public float WEED_TICK_TIME => weedTickTime;
    public float WEED_SPAWN_CHANCE => weedSpawnChance;
    public float WEED_GROWTH_SPEED => weedGrowthSpeed;

    /// <summary>
    /// WEED_PENALTY_SPEED is a multiplier that decreases the growth speed of the plant when there are weeds present.
    /// </summary>
    public float WEED_PENALTY_SPEED => weedPenaltySpeed;
    public int WATER_MULTIPLIER => waterMultiplier;
    public int MAX_WEED => maxWeed;
    public float WATER_EXISTING_TIME => waterExistingTime;

    /// <summary>
    /// waterBonusGrowthSpeed is a multiplier that increases the growth speed of the plant when it is watered.
    /// </summary>
    public float WATER_BONUS_GROWTH_SPEED => waterBonusGrowthSpeed;
    public int TOTAL_WATER_LEVELS => totalWaterLevels;
    #endregion
}
