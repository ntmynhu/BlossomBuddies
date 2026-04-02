using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D heightMap;
    [SerializeField] private Grid grid;

    [SerializeField] private float greyScaleOffset = 0.1f;
    [SerializeField] private float heightMultiplier = 10f;

    [SerializeField] private RuleTile3D ruleTile;
    [SerializeField] private GameObject defaultTile;

    private TileMapData[,] tiles;

    //[ContextMenu("Generate Map")]
    //public void GenerateMap()
    //{
    //    ClearOld();

    //    int width = heightMap.width;
    //    int height = heightMap.height;

    //    tiles = new TileMapData[width, height];

    //    Debug.Log(heightMap.GetPixel(0, 0).grayscale);
    //    Debug.Log(heightMap.GetPixel(64, 64).grayscale);

    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int z = 0; z < height; z++)
    //        {
    //            float alpha = heightMap.GetPixel(x, z).a;
    //            if (alpha < 0.1f) continue; // skip transparent pixel

    //            float h = heightMap.GetPixel(x, z).grayscale;

    //            // sample thêm pixel xung quanh
    //            float h1 = heightMap.GetPixel(x - 1, z).grayscale;
    //            float h2 = heightMap.GetPixel(x, z - 1).grayscale;
    //            float h3 = heightMap.GetPixel(x + 1, z).grayscale;
    //            float h4 = heightMap.GetPixel(x, z + 1).grayscale;

    //            float hmin = Mathf.Min(h, h1, h2, h3, h4);

    //            if ((h - hmin) > 0.01f)
    //            {
    //                float offset = (h - hmin) / greyScaleOffset;
    //                int count = Mathf.RoundToInt(offset);

    //                for (int i = 0; i <= count; i++)
    //                {
    //                    int heightLevel = Mathf.RoundToInt((hmin + greyScaleOffset * i) * heightMultiplier);
    //                    Debug.Log(heightLevel);
    //                    Vector3 worldPos = new Vector3(x, heightLevel, z);

    //                    GameObject tile = Instantiate(defaultTile, worldPos, Quaternion.identity, transform);

    //                    tiles[x, z] = new TileMapData
    //                    {
    //                        worldPosition = worldPos,
    //                        isOccupied = false,
    //                        tileObject = tile
    //                    };
    //                }
    //            }
    //            else
    //            {
    //                Vector3 worldPos = new Vector3(x, Mathf.RoundToInt(h * heightMultiplier), z);

    //                GameObject tile = Instantiate(defaultTile, worldPos, Quaternion.identity, transform);

    //                tiles[x, z] = new TileMapData
    //                {
    //                    worldPosition = worldPos,
    //                    isOccupied = false,
    //                    tileObject = tile
    //                };
    //            }
    //        }
    //    }
    //}

    [ContextMenu("Generate Ruled Map")]
    public void GenerateRuledMap()
    {
        ClearOld();

        int width = heightMap.width;
        int height = heightMap.height;

        tiles = new TileMapData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float alpha = heightMap.GetPixel(x, z).a;
                if (alpha < 0.1f) continue; // skip transparent pixel

                float h = heightMap.GetPixel(x, z).grayscale;
                int heightLevel = Mathf.RoundToInt(h * heightMultiplier);

                Vector3 worldPos = new Vector3(x, heightLevel, z);

                tiles[x, z] = new TileMapData
                {
                    worldPosition = worldPos,
                    height = heightLevel,
                    isOccupied = false,
                    tileObject = null
                };
            }
        }

        ApplyRuleTiles();
    }
    
    private void ApplyRuleTiles()
    {
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var tile = tiles[x, z];
                if (tile == null) continue;

                GameObject prefab = GetTilePrefab(x, z);

                GameObject obj = Instantiate(prefab, tile.worldPosition, Quaternion.identity, transform);
                tile.tileObject = obj;
            }
        }
    }

    private GameObject GetTilePrefab(int x, int z)
    {
        foreach (var rule in ruleTile.rules)
        {
            if (rule.prefab == null) continue;

            if (MatchRule(rule, x, z))
                return rule.prefab;
        }

        return defaultTile;
    }

    private bool MatchRule(TileRule rule, int x, int z)
    {
        Vector2Int[] offsets = new Vector2Int[]
        {
            new(-1,1), new(0,1), new(1,1),
            new(-1,0),           new(1,0),
            new(-1,-1), new(0,-1), new(1,-1)
        };

        for (int i = 0; i < 8; i++)
        {
            if (!Check(rule.neighbors[i], x, z, x + offsets[i].x, z + offsets[i].y))
                return false;
        }

        return true;
    }

    private bool Check(NeighborCondition cond, int x1, int z1, int x2, int z2)
    {
        Debug.Log($"Checking condition {cond} for ({x1},{z1}) and ({x2},{z2})");
        if (cond == NeighborCondition.Any) return true;

        bool same = IsSameHeight(x1, z1, x2, z2);
        return cond == NeighborCondition.Same ? same : !same;
    }

    private bool IsSameHeight(int x1, int z1, int x2, int z2)
    {
        if (x2 < 0 || z2 < 0 || x2 >= tiles.GetLength(0) || z2 >= tiles.GetLength(1))
            return false;

        if (tiles[x1, z1] == null || tiles[x2, z2] == null)
            return false;

        return tiles[x1, z1].height == tiles[x2, z2].height;
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
    public int height;
    public bool isOccupied;
    public GameObject tileObject;
}
