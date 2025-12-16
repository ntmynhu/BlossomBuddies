using UnityEngine;
using UnityEngine.InputSystem;

public class BathTub : PlayerDetect
{
    [SerializeField] private Transform petHolder;

    private void OnBathTubInteract()
    {
        if (isPlayerInRange)
        {
            ToolHandler toolHandler = GameManager.Instance.Player.GetComponent<ToolHandler>();
            PetStateManager currentPet = toolHandler.CurrentPet;

            if (currentPet != null)
            {
                toolHandler.OnPutDownPet(currentPet);

                currentPet.NavMeshAgent.enabled = false;
                currentPet.Rigidbody.isKinematic = true;
                currentPet.transform.SetParent(petHolder);

                currentPet.transform.localPosition = Vector3.zero;
                currentPet.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
