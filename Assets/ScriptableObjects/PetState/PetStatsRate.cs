using UnityEngine;

[CreateAssetMenu(fileName = "PetStatsRate", menuName = "Scriptable Objects/PetStatsRate")]
public class PetStatsRate : ScriptableObject
{
    public float EnergyRate;
    public float FoodRate;
    public float HappinessRate;
    public float CleanlinessRate;
}
