using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private ObjectData objectData;

    public ObjectData ObjectData => objectData;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        PlacementSystem.Instance.SetCurrentObjectData(objectData);
    }
}
