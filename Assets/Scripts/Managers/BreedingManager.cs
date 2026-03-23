using UnityEngine;
using System;
using UnityEngine.UI;

public class BreedingManager : Singleton<BreedingManager>
{
    [SerializeField] private GameObject breedingPanel;
    [SerializeField] private GameObject flowerPanelContent;
    [SerializeField] private FlowerInventorySlotUI breedingSlotUI;

    [SerializeField] private ParentSlot[] parentSlots;
    [SerializeField] private Button breedButton;

    private void Start()
    {
        foreach (var slot in parentSlots)
        {
            DeselectPlant(slot);
        }
    }

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
                FlowerInventorySlotUI slot = Instantiate(breedingSlotUI, flowerPanelContent.transform);
                
                int quantity = InventoryManager.Instance.GetItemQuantity(plant);
                slot.SetData(plant, quantity);
            }
        }
    }

    public void OnPlantSelected(PlantData selectedPlant)
    {
        // Handle plant selection for breeding
        Debug.Log($"Selected plant for breeding: {selectedPlant.name}");

        // Check for empty parent slot and assign the selected plant
        foreach (var slot in parentSlots)
        {
            if (slot.plantData == null)
            {
                slot.plantData = selectedPlant;
                slot.flowerImage.sprite = selectedPlant.icon;
                slot.flowerImage.gameObject.SetActive(true);
                slot.deselectButton.interactable = true;

                slot.deselectButton.onClick.RemoveAllListeners();
                slot.deselectButton.onClick.AddListener(() => DeselectPlant(slot));

                break;
            }
        }
    }

    private void DeselectPlant(ParentSlot slot)
    {
        slot.plantData = null;
        slot.flowerImage.sprite = null;
        slot.flowerImage.gameObject.SetActive(false);
        slot.deselectButton.interactable = false;
    }

    public void StartBreeding(PlantData plant1, PlantData plant2)
    {
        // Implement breeding logic here
        Debug.Log($"Breeding {plant1.name} with {plant2.name}");
    }
}

[Serializable]
public class ParentSlot
{
    public PlantData plantData;
    public Image flowerImage;
    public Button deselectButton;
}
