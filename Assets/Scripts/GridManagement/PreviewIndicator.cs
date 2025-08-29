using UnityEngine;

public class PreviewIndicator : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GameObject previewPrefab;

    private GameObject spawnObject;

    public void UpdateIndicator(GameObject prefab, Vector2Int size)
    {
        if (spawnObject != null)
        {
            Destroy(spawnObject);
        }
        
        spawnObject = Instantiate(prefab, previewPrefab.transform);
        
        float scaleY = cellIndicator.transform.localScale.y;
        cellIndicator.transform.localScale = new Vector3(size.x, scaleY, size.y);

        float posY = cellIndicator.transform.position.y;
        cellIndicator.transform.position = new Vector3(size.x / 2f, posY, size.y / 2f);
    }
}
