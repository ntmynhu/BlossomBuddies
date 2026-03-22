using UnityEngine;

[CreateAssetMenu(fileName = "WorldTimeConfig", menuName = "Scriptable Objects/WorldTimeConfig")]
public class WorldTimeConfig : ScriptableObject
{
    [Tooltip("Total realtime hours of a day in game")]
    public float timeOfDay = 24;

    [Tooltip("Number of hours in a day in game")]
    public float hoursInDay = 24;

    [Tooltip("Time in seconds to update lighting when playing game")]
    public float updateLigtingTime = 30f;
}