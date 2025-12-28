using UnityEngine;
using UnityEngine.InputSystem;

public class BathTub : PlayerDetect
{
    [SerializeField] private Transform petHolder;

    private PetStateManager currentPet;
    public PetStateManager CurrentPet => currentPet;

    public void OnBathTubInteract()
    {
        if (isPlayerInRange)
        {
            ToolHandler toolHandler = GameManager.Instance.Player.GetComponent<ToolHandler>();
            PetStateManager currentPet = toolHandler.CurrentPet;

            if (currentPet != null)
            {
                toolHandler.OnPutDownPet(currentPet);
                currentPet.ChangeState(currentPet.bathingState);

                currentPet.NavMeshAgent.enabled = false;
                currentPet.Rigidbody.isKinematic = true;
                currentPet.transform.SetParent(petHolder);

                currentPet.transform.localPosition = Vector3.zero;
                currentPet.transform.localRotation = Quaternion.identity;

                this.currentPet = currentPet;
            }
        }
    }

    public void OnPetEnter(PetStateManager pet)
    {
        toolHandler.OnPutDownPet(pet);
        pet.ChangeState(pet.bathingState);

        pet.NavMeshAgent.enabled = false;
        pet.Rigidbody.isKinematic = true;
        pet.transform.SetParent(petHolder);

        pet.transform.localPosition = Vector3.zero;
        pet.transform.localRotation = Quaternion.identity;

        this.currentPet = pet;
    }

    public void OnPetExitBathTub()
    {
        currentPet = null;
    }
}
