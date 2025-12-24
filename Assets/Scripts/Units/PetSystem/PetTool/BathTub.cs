using UnityEngine;
using UnityEngine.InputSystem;

public class BathTub : PlayerDetect
{
    [SerializeField] private Transform petHolder;

    private PetStateManager currentPet;
    public PetStateManager CurrentPet => currentPet;

    private void OnBathTubInteract()
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

    public void OnPetExitBathTub()
    {
        currentPet = null;
    }
}
