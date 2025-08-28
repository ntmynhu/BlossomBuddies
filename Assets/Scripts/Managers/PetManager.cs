using UnityEngine;

public class PetManager : Singleton<PetManager>
{
    [SerializeField] private Transform foodPosition;

    public Transform FoodPosition => foodPosition;
}
