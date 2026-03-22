using UnityEngine;

public class BreedingManager : Singleton<BreedingManager>
{
    [SerializeField] private GameObject breedingPanel;
    [SerializeField] private GameObject flowerPanelContent;
    [SerializeField] private BreedingSlotUI breedingSlotUI;

    private void Update()
    {
        HandleBreedingPanel();
    }

    private void HandleBreedingPanel()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBreedingPanel();
        }
    }

    private void ToggleBreedingPanel()
    {
        breedingPanel.SetActive(!breedingPanel.activeSelf);
        GameManager.Instance.SetCameraFrozen(breedingPanel.activeSelf);

        if (breedingPanel.activeSelf)
        {
            foreach (Transform child in flowerPanelContent.transform)
            {
                Destroy(child.gameObject);
            }

            // Populate breeding panel with player's plants
            var plantList = InventoryManager.Instance.GetInventoryObjectsByType<PlantData>();
            foreach (var plant in plantList)
            {
                BreedingSlotUI slot = Instantiate(breedingSlotUI, flowerPanelContent.transform);
                
                int quantity = InventoryManager.Instance.GetItemQuantity(plant);
                slot.SetData(plant, quantity);
            }
        }
    }

    public void OnPlantSelected(PlantData selectedPlant)
    {
        // Handle plant selection for breeding
        Debug.Log($"Selected plant for breeding: {selectedPlant.name}");
    }

    public void StartBreeding(PlantData plant1, PlantData plant2)
    {
        // Implement breeding logic here
        Debug.Log($"Breeding {plant1.name} with {plant2.name}");
    }
}
