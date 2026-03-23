using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerInventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image numberPanel;
    [SerializeField] private TextMeshProUGUI quantityText;

    private PlantData data;
    private Button button;

    public void SetData(PlantData newData, int quantity)
    {
        data = newData;

        PlantData plantData = data as PlantData;
        if (plantData != null && plantData.icon != null)
        {
            if (image != null)
            {
                image.sprite = plantData.icon;
            }
        }

        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
            numberPanel.gameObject.SetActive(true);
        }
        else
        {
            numberPanel.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        image = GetComponent<Image>();
    }

    private void OnClick()
    {
        BreedingManager.Instance.OnPlantSelected(data);
    }
}
