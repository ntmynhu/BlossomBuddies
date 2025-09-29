using System.Collections.Generic;
using UnityEngine;

public class WateredSoil : Tile
{
    [SerializeField] private List<Material> waterdMaterials;

    public int TotalStates => waterdMaterials.Count;

    public void ChangeMaterial(int index)
    {
        if (index < 0 || index >= waterdMaterials.Count) return;
        meshRenderer.material = waterdMaterials[index];
    }
}
