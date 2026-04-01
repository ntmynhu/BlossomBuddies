using UnityEngine;

[CreateAssetMenu(fileName = "LightingPreset", menuName = "ScriptableObjects/LightingPreset")]
public class LightingPreset : ScriptableObject
{
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;
}
