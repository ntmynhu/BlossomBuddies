using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileData[] tileData;
}

[Serializable]
public class TileData
{
    public TileType tileType;
    public Mesh mesh;
}

public enum TileType
{
    Grass,
    Soil,
    DarkSoil,
}
