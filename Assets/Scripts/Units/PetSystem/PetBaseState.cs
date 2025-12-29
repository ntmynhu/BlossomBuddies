using UnityEngine;

public abstract class PetBaseState
{
    public PetStatsRate StatsRate { get; set; }
    public abstract void EnterState(PetStateHandler cat);
    public virtual void UpdateState(PetStateHandler cat)
    {
        cat.Energy += StatsRate.EnergyRate * Time.deltaTime;
        cat.Happiness += StatsRate.HappinessRate * Time.deltaTime;
        cat.Food += StatsRate.FoodRate * Time.deltaTime;
        cat.Cleanliness += StatsRate.CleanlinessRate * Time.deltaTime;
    }
    public abstract void ExitState(PetStateHandler cat);
    public virtual void OnCollisionEnter(PetStateHandler cat, Collision collision) { }
    public virtual void OnTriggerEnter(PetStateHandler cat, Collider other) { }
    public virtual void OnTriggerStay(PetStateHandler cat, Collider other) { }
    public virtual void OnInteract(PetStateHandler cat) { }
}
