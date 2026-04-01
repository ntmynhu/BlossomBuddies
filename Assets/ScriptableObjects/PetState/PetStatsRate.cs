using UnityEngine;

[CreateAssetMenu(fileName = "PetStatsRate", menuName = "ScriptableObjects/PetStatsRate")]
public class PetStatsRate : ScriptableObject
{
    public float EnergyRate;
    public float FoodRate;
    public float HappinessRate;
    public float CleanlinessRate;
}
