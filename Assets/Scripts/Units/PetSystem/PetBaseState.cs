using UnityEngine;

public abstract class PetBaseState
{
    public PetStatsRate StatsRate { get; set; }
    public abstract void EnterState(PetStateManager cat);
    public virtual void UpdateState(PetStateManager cat)
    {
        cat.Energy += StatsRate.EnergyRate * Time.deltaTime;
        cat.Happiness += StatsRate.HappinessRate * Time.deltaTime;
        cat.Food += StatsRate.FoodRate * Time.deltaTime;
        cat.Cleanliness += StatsRate.CleanlinessRate * Time.deltaTime;
    }
    public abstract void ExitState(PetStateManager cat);
    public virtual void OnCollisionEnter(PetStateManager cat, Collision collision) { }
    public virtual void OnTriggerEnter(PetStateManager cat, Collider other) { }
    public virtual void OnTriggerStay(PetStateManager cat, Collider other) { }
}
