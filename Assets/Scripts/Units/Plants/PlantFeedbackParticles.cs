using UnityEngine;

/// <summary>
/// Placeholder "sparkle" FX above a plant, shown only while it meets its conditions and is
/// growing. Built procedurally at runtime and toggled from the plant's state each frame.
/// </summary>
public class PlantFeedbackParticles : MonoBehaviour
{
    private Plant _plant;
    private ParticleSystem _sparkle;

    private static Material _sharedMat;

    public static PlantFeedbackParticles Create(Plant plant)
    {
        var go = new GameObject("FeedbackFX");
        go.transform.SetParent(plant.transform, false);

        var fx = go.AddComponent<PlantFeedbackParticles>();
        fx._plant = plant;
        fx._sparkle = fx.BuildSparkle(go.transform);
        return fx;
    }

    private void Update()
    {
        if (_plant == null) { Destroy(gameObject); return; }

        bool thriving = !_plant.IsDead && !_plant.IsFullyGrown && _plant.AreStatsReady && _plant.IsGrowing;
        var emission = _sparkle.emission;
        if (emission.enabled != thriving) emission.enabled = thriving;
    }

    private ParticleSystem BuildSparkle(Transform parent)
    {
        var go = new GameObject("Sparkle");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(0f, 0.55f, 0f);

        var ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 40;
        main.playOnAwake = true;
        main.startLifetime = 0.85f;
        main.startSpeed = 0.2f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
        main.startColor = new Color(1f, 0.95f, 0.5f, 1f);
        main.gravityModifier = -0.12f; // drift upward

        var emission = ps.emission;
        emission.enabled = false; // Update() turns it on while thriving
        emission.rateOverTime = 9f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.35f;

        // Twinkle: fade in then out.
        var col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = MakeTwinkleGradient();

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.Facing;
        renderer.material = GetSharedMaterial();
        return ps;
    }

    private static Material GetSharedMaterial()
    {
        if (_sharedMat != null) return _sharedMat;

        const int size = 32;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float r = size * 0.5f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r)) / r;
                float a = Mathf.Clamp01(1f - d);
                a *= a;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;

        _sharedMat = new Material(Shader.Find("Sprites/Default"));
        _sharedMat.mainTexture = tex;
        return _sharedMat;
    }

    private static Gradient MakeTwinkleGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(0f, 1f),
            });
        return g;
    }
}
