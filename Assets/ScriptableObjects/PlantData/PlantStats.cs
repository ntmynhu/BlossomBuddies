using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantStats", menuName = "Scriptable Objects/PlantStats")]
public class PlantStats : ScriptableObject
{
    [Tooltip("Minimum and maximum values for the main stats of the plant, such as light, water, and nutrient levels.")]
    [SerializeField] private float minMainStatValue;
    [SerializeField] private float maxMainStatValue;

    [Tooltip("The time interval, in hours (IN GAME), for checking the main stats of the plant.")]
    [SerializeField] private float mainStatTick;

    [Header("Weed Stats")]
    [Tooltip("The time interval, in hours (IN GAME), checking for weed growth.")]
    [SerializeField] private float weedTickTime;

    [Tooltip("The chance for a weed to spawn every tick, expressed as a percentage (e.g., 0.5 for 0.5% chance).")]
    [SerializeField] private float weedSpawnChance;

    [Tooltip("The speed weeds grow every tick, in scale")]
    [SerializeField] private float weedGrowthSpeed;

    [Tooltip("The penalty speed applied to the plant's growth when weeds are present, per weed (e.g., 10%")]
    [SerializeField] private float weedPenaltySpeed;

    [Tooltip("The multiplier applied to the Weed Stats when watered (e.g., 3 means Weed Stats are increased by 3 times)")]
    [SerializeField] private int waterMultiplier;
    [SerializeField] private int maxWeed;

    [Header("Water Stats")]
    [Tooltip("The time, in hours (IN GAME), that water remains for the plant.")]
    [SerializeField] private float waterExistingTime;

    [Tooltip("The bonus growth speed applied to the plant when it is watered, in scale (e.g., 1.5 means 50% faster growth)")]
    [SerializeField] private float waterBonusGrowthSpeed; // xn growth speed
    [SerializeField] private int totalWaterLevels;

    [Tooltip("The value of the water stat (Every TICK) when the plant is watered with dark water and light water, respectively.")]
    [SerializeField] private float darkWaterValue;
    [SerializeField] private float lightWaterValue;

    [SerializeField] private List<TimeWaterValue> timeWaterDecreaseValues;

    #region Properties
    public float MIN_MAIN_STAT_VALUE => minMainStatValue;
    public float MAX_MAIN_STAT_VALUE => maxMainStatValue;

    public float MAIN_STAT_TICK => mainStatTick;

    public float WEED_TICK_TIME => weedTickTime;
    public float WEED_SPAWN_CHANCE => weedSpawnChance;
    public float WEED_GROWTH_SPEED => weedGrowthSpeed;
    public float WEED_PENALTY_SPEED => weedPenaltySpeed;
    public int WATER_MULTIPLIER => waterMultiplier;
    public int MAX_WEED => maxWeed;
    public float WATER_EXISTING_TIME => waterExistingTime;

    ///// <summary>
    ///// waterBonusGrowthSpeed is a multiplier that increases the growth speed of the plant when it is watered.
    ///// </summary>
    //public float WATER_BONUS_GROWTH_SPEED => waterBonusGrowthSpeed;
    public int TOTAL_WATER_LEVELS => totalWaterLevels;
    public float DARK_WATER_VALUE => darkWaterValue;
    public float LIGHT_WATER_VALUE => lightWaterValue;
    public List<TimeWaterValue> TIME_WATER_DECREASE_VALUES => timeWaterDecreaseValues;
    #endregion
}
