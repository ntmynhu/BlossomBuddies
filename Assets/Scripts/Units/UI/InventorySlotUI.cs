using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image numberPanel;
    [SerializeField] private TextMeshProUGUI quantityText;

    private BaseData data;
    private Button button;

    public void SetData(BaseData newData, int quantity)
    {
        data = newData;

        BaseData previewData = data as BaseData;
        if (previewData != null && previewData.icon != null)
        {
            if (image != null)
            {
                image.sprite = previewData.icon;
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
        InventoryManager.Instance.OnItemSelected(data);
    }
}
