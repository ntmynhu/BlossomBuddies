using UnityEngine;
using UnityEngine.UI;

public class FurnitureSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private ObjectData objectData;

    public ObjectData ObjectData => objectData;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        if (objectData != null && iconImage != null)
        {
            iconImage.sprite = objectData.icon;
        }
    }

    private void OnButtonClick()
    {
        PlacementSystem.Instance.SetCurrentObjectData(objectData);
    }
}
