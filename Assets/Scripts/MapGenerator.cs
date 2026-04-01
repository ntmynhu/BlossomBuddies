using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D heightMap;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Grid grid;

    [SerializeField] private float greyScaleOffset = 0.1f;
    [SerializeField] private float heightMultiplier = 10f;

    private TileMapData[,] tiles;

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        ClearOld();

        int width = heightMap.width;
        int height = heightMap.height;

        tiles = new TileMapData[width, height];

        Debug.Log(heightMap.GetPixel(0, 0).grayscale);
        Debug.Log(heightMap.GetPixel(64, 64).grayscale);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float alpha = heightMap.GetPixel(x, z).a;
                if (alpha < 0.1f) continue; // skip transparent pixel

                float h = heightMap.GetPixel(x, z).grayscale;

                // sample thêm pixel xung quanh
                float h1 = heightMap.GetPixel(x - 1, z).grayscale;
                float h2 = heightMap.GetPixel(x, z - 1).grayscale;
                float h3 = heightMap.GetPixel(x + 1, z).grayscale;
                float h4 = heightMap.GetPixel(x, z + 1).grayscale;

                float hmin = Mathf.Min(h, h1, h2, h3, h4);

                if ((h - hmin) > greyScaleOffset)
                {
                    float offset = (h - hmin) / greyScaleOffset;
                    int count = Mathf.RoundToInt(offset);

                    for (int i = 0; i < count; i++)
                    {
                        int heightLevel = Mathf.RoundToInt((hmin + greyScaleOffset * (i + 1)) * heightMultiplier);
                        Vector3 worldPos = new Vector3(x, heightLevel, z);

                        GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);

                        tiles[x, z] = new TileMapData
                        {
                            worldPosition = worldPos,
                            isOccupied = false,
                            tileObject = tile
                        };
                    }
                }
                else
                {
                    Vector3 worldPos = new Vector3(x, Mathf.RoundToInt(h * heightMultiplier), z);

                    GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);

                    tiles[x, z] = new TileMapData
                    {
                        worldPosition = worldPos,
                        isOccupied = false,
                        tileObject = tile
                    };
                }
            }
        }
    }
    
    private void ClearOld()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

public class TileMapData
{
    public Vector3 worldPosition;
    public bool isOccupied;
    public GameObject tileObject;
}
