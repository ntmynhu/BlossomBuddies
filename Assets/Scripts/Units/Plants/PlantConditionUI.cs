using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Little world-space indicators shown above a plant: three circular dots for Light (yellow),
/// Water (blue) and Nutrient (brown). Each dot fills from the bottom by its 0..1 percentage.
/// - Condition met  -> full bright colour.
/// - Condition lacking -> dimmed colour.
/// - Lacking but trending up (e.g. daytime for light) -> blinks while it fills.
/// Built entirely at runtime; created/toggled from Plant.ToggleConditionUI().
/// </summary>
public class PlantConditionUI : MonoBehaviour
{
    private const float HeightOffset = 1.8f;
    private const float CanvasScale = 0.005f;
    private const float BlinkSpeed = 3f;
    private const float DimBrightness = 0.4f;

    private static readonly PlantMainStatsType[] Types =
    {
        PlantMainStatsType.Light, PlantMainStatsType.Water, PlantMainStatsType.Nutrient
    };

    private static readonly Color[] BaseColors =
    {
        new Color(1f, 0.85f, 0.2f),  // Light  - yellow
        new Color(0.3f, 0.6f, 1f),   // Water  - blue
        new Color(0.6f, 0.4f, 0.2f), // Nutrient - brown
    };

    private static Sprite _circleSprite;

    private Plant _plant;
    private Image[] _fills;
    private bool[] _hasCustomSprite;
    private Camera _cam;

    public static PlantConditionUI Create(Plant plant)
    {
        var go = new GameObject("ConditionUI");
        go.transform.SetParent(plant.transform, false);
        // Offset to the grid-cell center (plant sits at the cell corner) and lift above it.
        go.transform.localPosition = new Vector3(0.5f, HeightOffset, 0.5f);

        var ui = go.AddComponent<PlantConditionUI>();
        ui._plant = plant;
        ui.Build();
        return ui;
    }

    public void ToggleVisible() => gameObject.SetActive(!gameObject.activeSelf);

    private void Build()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        var rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(330, 110);
        rt.localScale = Vector3.one * CanvasScale;

        var row = gameObject.AddComponent<HorizontalLayoutGroup>();
        row.spacing = 20;
        row.childAlignment = TextAnchor.MiddleCenter;
        row.childControlWidth = true;
        row.childControlHeight = true;
        row.childForceExpandWidth = false;
        row.childForceExpandHeight = false;

        _fills = new Image[Types.Length];
        _hasCustomSprite = new bool[Types.Length];
        for (int i = 0; i < Types.Length; i++)
        {
            Sprite custom = _plant != null ? _plant.GetConditionIcon(Types[i]) : null;
            Sprite customBg = _plant != null ? _plant.GetConditionBgIcon(Types[i]) : null;
            _hasCustomSprite[i] = custom != null;
            _fills[i] = BuildIcon(transform, BaseColors[i], custom, customBg);
        }

        _cam = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
    }

    private Image BuildIcon(Transform parent, Color baseColor, Sprite customSprite, Sprite customBg)
    {
        var icon = new GameObject("Icon", typeof(RectTransform));
        icon.transform.SetParent(parent, false);
        // ~25% smaller than the original 90.
        const float IconSize = 67.5f;
        var le = icon.AddComponent<LayoutElement>();
        le.preferredWidth = le.preferredHeight = IconSize;
        le.minWidth = le.minHeight = IconSize;

        Sprite shape = customSprite != null ? customSprite : GetCircleSprite();

        // Background (the empty part). Prefer a dedicated gray sprite; otherwise dim the shape.
        var bg = NewImage(icon.transform, "Bg", customBg != null ? customBg : shape);
        if (customBg != null)
            bg.color = Color.white;                 // use the gray sprite's own colors
        else if (customSprite != null)
            bg.color = new Color(1f, 1f, 1f, 0.25f);
        else
            bg.color = new Color(baseColor.r * 0.25f, baseColor.g * 0.25f, baseColor.b * 0.25f, 0.55f);

        // Foreground fill that rises from the bottom.
        var fill = NewImage(icon.transform, "Fill", shape);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Vertical;
        fill.fillOrigin = (int)Image.OriginVertical.Bottom;
        fill.fillAmount = 0f;
        // Custom sprites keep their own colors (white tint); the default circle is tinted per stat.
        fill.color = customSprite != null ? Color.white : baseColor;
        return fill;
    }

    private static Image NewImage(Transform parent, string name, Sprite sprite)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var stretch = go.GetComponent<RectTransform>();
        stretch.anchorMin = Vector2.zero;
        stretch.anchorMax = Vector2.one;
        stretch.offsetMin = Vector2.zero;
        stretch.offsetMax = Vector2.zero;

        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;   // keep sprite proportions, no stretching
        img.raycastTarget = false;
        return img;
    }

    private void LateUpdate()
    {
        if (_plant == null) { Destroy(gameObject); return; }

        // Billboard: face the camera.
        if (_cam == null) _cam = Camera.main;
        if (_cam != null)
            transform.rotation = _cam.transform.rotation;

        for (int i = 0; i < _fills.Length; i++)
        {
            var info = _plant.GetConditionInfo(Types[i]);
            _fills[i].fillAmount = info.fill;

            float brightness;
            if (info.sufficient)
                brightness = 1f;
            else if (info.increasing)
                brightness = Mathf.Lerp(DimBrightness, 0.95f, Mathf.PingPong(Time.time * BlinkSpeed, 1f));
            else
                brightness = DimBrightness;

            // Custom sprites use their own colors (white tint); the default circle is tinted per stat.
            var tint = _hasCustomSprite[i] ? Color.white : BaseColors[i];
            var c = tint * brightness;
            c.a = 1f;
            _fills[i].color = c;
        }
    }

    // A soft white circle, generated once and reused (tinted per icon).
    private static Sprite GetCircleSprite()
    {
        if (_circleSprite != null) return _circleSprite;

        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float r = size * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                float a = Mathf.Clamp01((r - dist) / 1.5f); // 1 inside, smooth edge
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        _circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return _circleSprite;
    }
}
