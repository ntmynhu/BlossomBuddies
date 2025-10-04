using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image image;

    private ScriptableObject data;
    public ScriptableObject Data { get { return data; } set { data = value; } }

    private Button button;

    public void SetData(ScriptableObject newData)
    {
        data = newData;

        PreviewData previewData = data as PreviewData;
        if (previewData != null && previewData.icon != null)
        {
            if (image != null)
            {
                image.sprite = previewData.icon;
            }
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
