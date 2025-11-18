using System;
using UnityEngine;

public class PreviewIndicator : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GameObject previewPrefab;

    [SerializeField] private Color validColor  = Color.white;
    [SerializeField] private Color invalidColor = Color.red;

    private GameObject spawnObject;
    private Renderer cellRenderer;

    private void Start()
    {
        cellRenderer = cellIndicator.GetComponent<Renderer>();
    }

    public void UpdateIndicator(GameObject prefab, Vector2Int size)
    {
        if (spawnObject != null)
        {
            Destroy(spawnObject);
        }

        if (prefab != null)
        {
            spawnObject = Instantiate(prefab, previewPrefab.transform);
        }

        float scaleY = cellIndicator.transform.localScale.y;
        cellIndicator.transform.localScale = new Vector3(size.x, scaleY, size.y);

        cellIndicator.transform.localPosition = new Vector3(size.x / 2f, 0, size.y / 2f);

        Debug.Log($"Updated indicator to size {size} at position {cellIndicator.transform.localPosition}");
    }

    public void HidePreviewObject(bool value)
    {
        previewPrefab.SetActive(!value);
    }

    public void SetValid(bool isValid)
    {
        cellRenderer.material.color = isValid ? validColor : invalidColor;
    }
}
