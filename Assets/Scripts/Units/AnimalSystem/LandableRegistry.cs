using System.Collections.Generic;
using UnityEngine;

public class LandableRegistry : Singleton<LandableRegistry>
{
    private readonly List<LandableAutoRegister> landables = new();
    public List<LandableAutoRegister> Landables => landables;

    public void Register(LandableAutoRegister plane)
    {
        if (plane != null && !landables.Contains(plane)) landables.Add(plane);
    }

    public void Unregister(LandableAutoRegister plane)
    {
        if (plane != null) landables.Remove(plane);
    }
}
