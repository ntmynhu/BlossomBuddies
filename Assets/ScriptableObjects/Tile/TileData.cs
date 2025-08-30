using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public ObjectData mainObject;
    public GameObject tile_full;
    public GameObject tile_quarter_1;
    public GameObject tile_quarter_1_3;
    public GameObject tile_quarter_1_2_3;
    public GameObject tile_half;
    public GameObject other_tile_full;
}