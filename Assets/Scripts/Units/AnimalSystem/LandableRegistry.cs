using System.Collections.Generic;
using System;
using UnityEngine;

public class LandableRegistry : Singleton<LandableRegistry>
{
    private readonly Dictionary<LandableType, List<LandableAutoRegister>> landables = new();
    public Dictionary<LandableType, List<LandableAutoRegister>> Landables => landables;

    protected override void Awake()
    {
        base.Awake();

        foreach (LandableType type in Enum.GetValues(typeof(LandableType)))
        {
            landables[type] = new List<LandableAutoRegister>();
        }
    }

    public void Register(LandableAutoRegister plane)
    {
        landables[plane.LandableType].Add(plane);
    }

    public void Unregister(LandableAutoRegister plane)
    {
        landables[plane.LandableType].Remove(plane);
    }
}

[Serializable]
public enum LandableType
{
    Bird,
    Insect,
}
