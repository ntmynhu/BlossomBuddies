using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private ThirdPersonCameraController thirdPersonCameraController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Inventory opened");
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            thirdPersonCameraController.SetMobileController(inventoryPanel.activeSelf);

            if (inventoryPanel.activeSelf)
            {
                PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.FurnitureState, inventorySlots[0].ObjectData);
            }
            else
            {
                PlacementSystem.Instance.SwitchState(PlacementSystem.Instance.NormalState, null);
            }
        }

        if (inventoryPanel.activeSelf)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                PlacementSystem.Instance.TriggerAction();
            }
        }
    }
}
