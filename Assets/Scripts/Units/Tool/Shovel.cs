using UnityEngine;

public class Shovel : Tool
{
    [SerializeField] private ObjectData spawnObject;

    public override void UseTool()
    {
        base.UseTool();

        PlacementSystem.Instance.PlaceObject(spawnObject.ID);
    }
}
