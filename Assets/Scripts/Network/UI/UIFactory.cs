using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// Small helpers to build uGUI + TextMeshPro elements at runtime, so the login and
    /// market screens work without any manual scene wiring. Uses TMP_DefaultControls so
    /// input fields / buttons get their correct internal hierarchy.
    /// </summary>
    public static class UIFactory
    {
        private static readonly TMP_DefaultControls.Resources Res = new TMP_DefaultControls.Resources();

        public static Canvas CreateOverlayCanvas(string name)
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        public static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        public static RectTransform FullScreen(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static GameObject Panel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = color;
            return go;
        }

        public static VerticalLayoutGroup Vertical(GameObject go, int spacing = 8, int pad = 16)
        {
            var v = go.AddComponent<VerticalLayoutGroup>();
            v.spacing = spacing;
            v.padding = new RectOffset(pad, pad, pad, pad);
            v.childControlWidth = true;
            v.childControlHeight = true;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            return v;
        }

        public static HorizontalLayoutGroup Horizontal(GameObject go, int spacing = 8, int pad = 0)
        {
            var h = go.AddComponent<HorizontalLayoutGroup>();
            h.spacing = spacing;
            h.padding = new RectOffset(pad, pad, pad, pad);
            h.childControlWidth = true;
            h.childControlHeight = true;
            h.childForceExpandWidth = false;
            h.childForceExpandHeight = false;
            h.childAlignment = TextAnchor.MiddleLeft;
            return h;
        }

        public static TextMeshProUGUI Text(Transform parent, string text, int size = 28,
            TextAlignmentOptions align = TextAlignmentOptions.Left, FontStyles style = FontStyles.Normal)
        {
            var go = new GameObject("Text", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.alignment = align;
            t.fontStyle = style;
            t.color = new Color(0.15f, 0.12f, 0.1f);
            t.raycastTarget = false;
            return t;
        }

        public static TMP_InputField Input(Transform parent, string placeholder,
            TMP_InputField.ContentType content = TMP_InputField.ContentType.Standard)
        {
            var go = TMP_DefaultControls.CreateInputField(Res);
            go.transform.SetParent(parent, false);
            var input = go.GetComponent<TMP_InputField>();
            input.contentType = content;
            if (input.placeholder is TMP_Text ph) { ph.text = placeholder; ph.color = new Color(0.5f, 0.5f, 0.5f); }
            if (input.textComponent != null) input.textComponent.color = Color.black;
            go.GetComponent<Image>().color = Color.white;
            SetHeight(go, 44);
            return input;
        }

        public static TMP_Dropdown Dropdown(Transform parent)
        {
            var go = TMP_DefaultControls.CreateDropdown(Res);
            go.transform.SetParent(parent, false);
            var dd = go.GetComponent<TMP_Dropdown>();
            dd.ClearOptions();
            go.GetComponent<Image>().color = Color.white;
            if (dd.captionText != null) dd.captionText.color = Color.black;
            if (dd.itemText != null) dd.itemText.color = Color.black;
            SetHeight(go, 44);
            return dd;
        }

        public static Button Button(Transform parent, string label, UnityAction onClick, Color? color = null)
        {
            var go = TMP_DefaultControls.CreateButton(Res);
            go.transform.SetParent(parent, false);
            var btn = go.GetComponent<Button>();
            if (onClick != null) btn.onClick.AddListener(onClick);
            go.GetComponent<Image>().color = color ?? new Color(0.55f, 0.78f, 0.55f);
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) { txt.text = label; txt.color = Color.white; txt.fontSize = 24; }
            SetHeight(go, 44);
            return btn;
        }

        public static Image Image(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = color;
            return img;
        }

        public static GridLayoutGroup Grid(GameObject go, Vector2 cell, Vector2 spacing, int columns, int pad = 0)
        {
            var g = go.AddComponent<GridLayoutGroup>();
            g.cellSize = cell;
            g.spacing = spacing;
            g.padding = new RectOffset(pad, pad, pad, pad);
            g.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            g.constraintCount = columns;
            g.childAlignment = TextAnchor.UpperCenter;
            return g;
        }

        public static void SetHeight(GameObject go, float height)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            le.minHeight = height;
            le.preferredHeight = height;
        }

        public static void SetWidth(GameObject go, float width)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            le.minWidth = width;
            le.preferredWidth = width;
            le.flexibleWidth = 0;
        }

        // A centered fixed-size box inside a full-screen parent.
        public static GameObject CenterBox(Transform parent, string name, Vector2 size, Color color)
        {
            var go = Panel(parent, name, color);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            return go;
        }
    }
}
