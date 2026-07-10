using BlossomBuddies.Network;
using BlossomBuddies.Network.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace BlossomBuddies.EditorTools
{
    /// <summary>
    /// Builds the online Canvas (Login + Market) as real, editable scene objects and wires all
    /// references. Run once from the menu; then tweak everything visually in the Inspector.
    /// Re-running rebuilds a fresh "OnlineCanvas" (delete the old one first if present).
    /// </summary>
    public static class OnlineUIBuilder
    {
        private const string RowPrefabPath = "Assets/Prefabs/UI/MarketRow.prefab";

        [MenuItem("Tools/Online/Build Login + Market UI")]
        public static void BuildAll()
        {
            var canvas = UIFactory.CreateOverlayCanvas("OnlineCanvas");
            UIFactory.EnsureEventSystem();

            var loginPanel = canvas.gameObject.AddComponent<LoginPanel>();
            var marketPanel = canvas.gameObject.AddComponent<MarketPanel>();

            BuildLogin(canvas.transform, loginPanel);
            var rowPrefab = BuildRowPrefab();
            BuildMarket(canvas.transform, marketPanel, rowPrefab);

            WireBootstrap(canvas);

            EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
            Selection.activeGameObject = canvas.gameObject;
            Debug.Log("[OnlineUIBuilder] Built OnlineCanvas with Login + Market. Save the scene to keep it.");
        }

        // ---------- Login ----------

        private static void BuildLogin(Transform canvas, LoginPanel panel)
        {
            var root = UIFactory.Panel(canvas, "LoginRoot", new Color(0f, 0f, 0f, 0.6f));
            UIFactory.FullScreen(root);

            var box = UIFactory.CenterBox(root.transform, "Box", new Vector2(520, 460),
                new Color(0.98f, 0.96f, 0.9f, 1f));
            UIFactory.Vertical(box, 12, 24);

            UIFactory.Text(box.transform, "Blossom Buddies", 40, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.Text(box.transform, "Sign in to trade with other players", 20, TextAlignmentOptions.Center);

            var username = UIFactory.Input(box.transform, "Username");
            var password = UIFactory.Input(box.transform, "Password", TMP_InputField.ContentType.Password);
            var loginBtn = UIFactory.Button(box.transform, "Login", null);
            var registerBtn = UIFactory.Button(box.transform, "Register", null, new Color(0.55f, 0.7f, 0.9f));
            var status = UIFactory.Text(box.transform, "", 18, TextAlignmentOptions.Center);
            status.color = new Color(0.7f, 0.2f, 0.2f);

            var so = new SerializedObject(panel);
            so.FindProperty("panelRoot").objectReferenceValue = root;
            so.FindProperty("usernameInput").objectReferenceValue = username;
            so.FindProperty("passwordInput").objectReferenceValue = password;
            so.FindProperty("loginButton").objectReferenceValue = loginBtn;
            so.FindProperty("registerButton").objectReferenceValue = registerBtn;
            so.FindProperty("statusText").objectReferenceValue = status;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ---------- Market ----------

        private static void BuildMarket(Transform canvas, MarketPanel panel, MarketRowUI rowPrefab)
        {
            var root = UIFactory.Panel(canvas, "MarketRoot", new Color(0f, 0f, 0f, 0.55f));
            UIFactory.FullScreen(root);

            var box = UIFactory.CenterBox(root.transform, "Box", new Vector2(920, 720),
                new Color(0.98f, 0.96f, 0.9f, 1f));
            UIFactory.Vertical(box, 10, 20);

            // Header
            var header = Row(box.transform, "Header", 48);
            var title = UIFactory.Text(header.transform, "Marketplace", 34, TextAlignmentOptions.Left, FontStyles.Bold);
            title.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            var closeBtn = UIFactory.Button(header.transform, "Close", null, new Color(0.85f, 0.5f, 0.5f));
            UIFactory.SetWidth(closeBtn.gameObject, 120);

            // Tabs
            var tabs = Row(box.transform, "Tabs", 44);
            var allBtn = UIFactory.Button(tabs.transform, "All listings", null);
            var myBtn = UIFactory.Button(tabs.transform, "My listings", null, new Color(0.55f, 0.7f, 0.9f));
            var refreshBtn = UIFactory.Button(tabs.transform, "Refresh", null, new Color(0.7f, 0.7f, 0.7f));

            // Form
            var form = Row(box.transform, "Form", 48);
            var itemInput = UIFactory.Input(form.transform, "Item id (e.g. 1003_BB)");
            itemInput.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            var qtyInput = UIFactory.Input(form.transform, "Qty", TMP_InputField.ContentType.IntegerNumber);
            UIFactory.SetWidth(qtyInput.gameObject, 90);
            var priceInput = UIFactory.Input(form.transform, "Price", TMP_InputField.ContentType.IntegerNumber);
            UIFactory.SetWidth(priceInput.gameObject, 90);
            var sellBtn = UIFactory.Button(form.transform, "Sell", null, new Color(0.55f, 0.78f, 0.55f));
            UIFactory.SetWidth(sellBtn.gameObject, 120);

            // Scroll list
            var content = CreateScrollView(box.transform);

            // Status
            var status = UIFactory.Text(box.transform, "", 18, TextAlignmentOptions.Left);
            UIFactory.SetHeight(status.gameObject, 26);

            var so = new SerializedObject(panel);
            so.FindProperty("overlayRoot").objectReferenceValue = root;
            so.FindProperty("listContent").objectReferenceValue = content;
            so.FindProperty("rowPrefab").objectReferenceValue = rowPrefab;
            so.FindProperty("itemInput").objectReferenceValue = itemInput;
            so.FindProperty("qtyInput").objectReferenceValue = qtyInput;
            so.FindProperty("priceInput").objectReferenceValue = priceInput;
            so.FindProperty("sellButton").objectReferenceValue = sellBtn;
            so.FindProperty("allTabButton").objectReferenceValue = allBtn;
            so.FindProperty("myTabButton").objectReferenceValue = myBtn;
            so.FindProperty("refreshButton").objectReferenceValue = refreshBtn;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("status").objectReferenceValue = status;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static MarketRowUI BuildRowPrefab()
        {
            var go = UIFactory.Panel(null, "MarketRow", new Color(1f, 1f, 1f, 0.9f));
            UIFactory.Horizontal(go, 10, 6);
            UIFactory.SetHeight(go, 52);

            var label = UIFactory.Text(go.transform, "Item", 22);
            label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            var statusLabel = UIFactory.Text(go.transform, "", 20);
            var actionBtn = UIFactory.Button(go.transform, "Buy", null, new Color(0.55f, 0.78f, 0.55f));
            UIFactory.SetWidth(actionBtn.gameObject, 120);
            var actionLabel = actionBtn.GetComponentInChildren<TextMeshProUGUI>();

            var row = go.AddComponent<MarketRowUI>();
            var so = new SerializedObject(row);
            so.FindProperty("label").objectReferenceValue = label;
            so.FindProperty("statusLabel").objectReferenceValue = statusLabel;
            so.FindProperty("actionButton").objectReferenceValue = actionBtn;
            so.FindProperty("actionButtonLabel").objectReferenceValue = actionLabel;
            so.ApplyModifiedPropertiesWithoutUndo();

            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/UI");
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, RowPrefabPath);
            Object.DestroyImmediate(go);
            return prefab.GetComponent<MarketRowUI>();
        }

        // ---------- Helpers ----------

        private static GameObject Row(Transform parent, string name, float height)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            UIFactory.Horizontal(go, 10);
            UIFactory.SetHeight(go, height);
            return go;
        }

        private static RectTransform CreateScrollView(Transform parent)
        {
            var scroll = new GameObject("Scroll", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
            scroll.transform.SetParent(parent, false);
            scroll.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
            scroll.AddComponent<LayoutElement>().flexibleHeight = 1;

            var sr = scroll.GetComponent<ScrollRect>();
            sr.horizontal = false; sr.vertical = true;
            sr.movementType = ScrollRect.MovementType.Clamped;
            sr.scrollSensitivity = 24;

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(scroll.transform, false);
            var vrt = UIFactory.FullScreen(viewport);
            vrt.pivot = new Vector2(0, 1);
            viewport.GetComponent<Mask>().showMaskGraphic = false;

            var contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(viewport.transform, false);
            var crt = contentGo.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 1);
            crt.anchorMax = new Vector2(1, 1);
            crt.pivot = new Vector2(0.5f, 1);
            crt.sizeDelta = Vector2.zero;

            var vlg = contentGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 6;
            vlg.padding = new RectOffset(8, 8, 8, 8);
            vlg.childControlWidth = true; vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

            contentGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            sr.viewport = vrt;
            sr.content = crt;
            return crt;
        }

        private static void WireBootstrap(Canvas canvas)
        {
            var boot = Object.FindObjectOfType<NetworkBootstrap>();
            if (boot == null)
            {
                var go = new GameObject("_Online");
                boot = go.AddComponent<NetworkBootstrap>();
                var bso = new SerializedObject(boot);
                bso.FindProperty("sceneToLoadAfterLogin").stringValue = "LoadingScene";
                bso.ApplyModifiedPropertiesWithoutUndo();
            }
            if (boot.GetComponent<ApiClient>() == null) boot.gameObject.AddComponent<ApiClient>();
            if (boot.GetComponent<SessionManager>() == null) boot.gameObject.AddComponent<SessionManager>();
            if (boot.GetComponent<ServerSyncManager>() == null) boot.gameObject.AddComponent<ServerSyncManager>();

            var so = new SerializedObject(boot);
            so.FindProperty("persistentCanvas").objectReferenceValue = canvas;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                var name = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }
}
