using UnityEngine;
using System;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class BreedingManager : Singleton<BreedingManager>
{
    [SerializeField] private GameObject breedingPanel;
    [SerializeField] private GameObject flowerPanelContent;
    [SerializeField] private FlowerInventorySlotUI breedingSlotUI;

    [SerializeField] private ParentSlot[] parentSlots;
    [SerializeField] private Button breedButton;

    [Header("Breeding Result")]
    [SerializeField] private GameObject breedingResultPanel;
    [SerializeField] private Image resultFlowerImage;
    [SerializeField] private TMPro.TMP_Text resultSeedCountText;
    [SerializeField] private Button resultCloseButton;

    [Header("Breeding Data")]
    [SerializeField] private List<AlenColor> alenColors;
    [SerializeField] private List<BreedingPool> breedingPools;

    private void Start()
    {
        foreach (var slot in parentSlots)
        {
            DeselectPlant(slot);
        }

        breedButton.onClick.AddListener(CheckBreeding);
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
        GameManager.Instance.SetMovementFrozen(breedingPanel.activeSelf);

        if (breedingPanel.activeSelf)
        {
            RefreshFlowerList();
        }
    }

    // Rebuilds the list of the player's plants with their current quantities.
    private void RefreshFlowerList()
    {
        foreach (Transform child in flowerPanelContent.transform)
        {
            Destroy(child.gameObject);
        }

        var plantList = InventoryManager.Instance.GetInventoryObjectsByType<PlantData>();
        foreach (var plant in plantList)
        {
            FlowerInventorySlotUI slot = Instantiate(breedingSlotUI, flowerPanelContent.transform);

            int quantity = InventoryManager.Instance.GetItemQuantity(plant);
            slot.SetData(plant, quantity);
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

                slot.alenPanel.SetActive(true);
                for (int i = 0; i < slot.alenColorImages.Count(); i++)
                {
                    AlenType type = slot.plantData.alenTypes[i];
                    Color color = GetAlenColor(type);

                    slot.alenColorImages[i].color = color;
                }

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

        slot.alenPanel.SetActive(false);
    }

    private void CheckBreeding()
    { 
        bool canBreed = parentSlots.All(slot => slot.plantData != null);
        if (!canBreed)
        {
            Debug.Log("Please select two plants to breed.");
            return;
        }

        var plant1 = parentSlots[0].plantData;
        var plant2 = parentSlots[1].plantData;

        // Make sure the player actually owns the selected parents (needs 2 if both are the
        // same plant type) before consuming them.
        int need1 = plant1 == plant2 ? 2 : 1;
        if (InventoryManager.Instance.GetItemQuantity(plant1) < need1)
        {
            Debug.Log("Not enough flowers to breed.");
            return;
        }

        StartBreeding(plant1, plant2);
    }

    public void StartBreeding(PlantData plant1, PlantData plant2)
    {
        Debug.Log($"Breeding {plant1.name} with {plant2.name}");

        // Consume the two parent flowers.
        InventoryManager.Instance.RemoveItem(plant1);
        InventoryManager.Instance.RemoveItem(plant2);

        List<AlenType> combinedAlenTypes = plant1.alenTypes.Concat(plant2.alenTypes).ToList();

        // Generate all possible combinations of AlenTypes for the offspring
        List<List<AlenType>> hybridAlenTypes = new List<List<AlenType>>();
        for (int i = 0; i < combinedAlenTypes.Count; i++)
        {
            for (int j = 0; j < combinedAlenTypes.Count; j++)
            {
                if (i != j)
                {
                    hybridAlenTypes.Add(new List<AlenType> { combinedAlenTypes[i], combinedAlenTypes[j] });
                }
            }
        }

        // Debug all hybrid combinations
        foreach (var combo in hybridAlenTypes)
        {
            Debug.Log($"Possible offspring AlenTypes: {combo[0]}, {combo[1]}");
        }

        // Choose a random combination for the offspring
        List<AlenType> offspringAlenTypes = hybridAlenTypes[UnityEngine.Random.Range(0, hybridAlenTypes.Count)];
        Debug.Log($"Chosen offspring AlenTypes: {offspringAlenTypes[0]}, {offspringAlenTypes[1]}");

        // Find a matching breeding pool based on parent plants
        BreedingPool matchingPool = breedingPools.FirstOrDefault(pool =>
            pool.plantDatas.Contains(plant1) && pool.plantDatas.Contains(plant2));

        PlantData offspringPlant = null;
        if (matchingPool != null)
        {
            foreach (var plant in matchingPool.plantDatas)
            {
                bool isAlenMatch = plant.alenTypes.OrderBy(a => a).SequenceEqual(offspringAlenTypes.OrderBy(a => a));

                if (isAlenMatch)
                {
                    offspringPlant = plant;
                    Debug.Log($"Found matching offspring plant: {offspringPlant.name}");
                    break;
                }
            }
        }

        if (matchingPool == null || offspringPlant == null)
        {
            Debug.Log("No matching breeding pool or offspring plant found. Breeding failed.");
            // Choose a random plant from parent plants as fallback
            offspringPlant = UnityEngine.Random.value < 0.5f ? plant1 : plant2;
        }

        // Parents are used up: clear their slots and refresh the flower list quantities.
        foreach (var slot in parentSlots)
            DeselectPlant(slot);
        RefreshFlowerList();

        // Breeding yields a random 1-3 seeds.
        int seedCount = UnityEngine.Random.Range(1, 4);

        // Display breeding result
        breedingResultPanel.SetActive(true);
        resultFlowerImage.sprite = offspringPlant.seedData.icon;
        if (resultSeedCountText != null) resultSeedCountText.text = $"x{seedCount}";

        resultCloseButton.onClick.RemoveAllListeners();
        resultCloseButton.onClick.AddListener(() =>
        {
            for (int i = 0; i < seedCount; i++)
                InventoryManager.Instance.AddItem(offspringPlant.seedData);

            RefreshFlowerList();
            breedingResultPanel.SetActive(false);
        });
    }

    public Color GetAlenColor(AlenType alenType)
    {
        foreach (var alenColor in alenColors)
        {
            if (alenColor.alen == alenType)
            {
                return alenColor.color;
            }
        }

        Debug.Log("Do not find color for type: " + alenType);
        return Color.black;
    }
}

[Serializable]
public class AlenColor
{
    public AlenType alen;
    public Color color;
}

[Serializable]
public class ParentSlot
{
    public PlantData plantData;
    public Image flowerImage;
    public Button deselectButton;
    public GameObject alenPanel;
    public Image[] alenColorImages;
}

[Serializable]
public class BreedingPool
{
    public PlantData[] plantDatas;
}
