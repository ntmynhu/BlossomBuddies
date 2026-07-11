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

        // Clears the saved JWT so the next launch shows the login screen (test multiple accounts).
        [MenuItem("Tools/Online/Clear Saved Login")]
        public static void ClearSavedLogin()
        {
            PlayerPrefs.DeleteKey("bb_auth_token");
            PlayerPrefs.DeleteKey("bb_auth_username");
            PlayerPrefs.Save();
            Debug.Log("[OnlineUIBuilder] Cleared saved login. Next play will show the login screen.");
        }

        // Run this in the LoadingScene: builds the login gate (persists into the game).
        [MenuItem("Tools/Online/Build Login Gate (run in LoadingScene)")]
        public static void BuildLoginGate()
        {
            var existing = GameObject.Find("OnlineCanvas");
            if (existing != null) Object.DestroyImmediate(existing);

            var canvas = UIFactory.CreateOverlayCanvas("OnlineCanvas");
            UIFactory.EnsureEventSystem();

            var loginPanel = canvas.gameObject.AddComponent<LoginPanel>();
            BuildLogin(canvas.transform, loginPanel);

            WireBootstrap(canvas); // network singletons + keep the login canvas across scenes

            EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
            Selection.activeGameObject = canvas.gameObject;
            Debug.Log("[OnlineUIBuilder] Built login gate (OnlineCanvas) in LoadingScene.");
        }

        // Run this in the MainScene: builds the in-game HUD (market/shop/inventory + logout).
        [MenuItem("Tools/Online/Build Game HUD (run in MainScene)")]
        public static void BuildGameHud()
        {
            var existing = GameObject.Find("GameHudCanvas");
            if (existing != null) Object.DestroyImmediate(existing);

            var canvas = UIFactory.CreateOverlayCanvas("GameHudCanvas");
            UIFactory.EnsureEventSystem();

            var marketPanel = canvas.gameObject.AddComponent<MarketPanel>();
            var sessionUI = canvas.gameObject.AddComponent<SessionUI>();

            var rowPrefab = BuildRowPrefab();
            BuildMarket(canvas.transform, marketPanel, rowPrefab);
            BuildLogout(canvas.transform, sessionUI);
            BuildHud(canvas.transform, sessionUI);

            EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
            Selection.activeGameObject = canvas.gameObject;
            Debug.Log("[OnlineUIBuilder] Built Game HUD (GameHudCanvas) in the current scene.");
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

        private static void BuildLogout(Transform canvas, SessionUI session)
        {
            // Top-right bar: [username] [Logout], shown only while logged in.
            var root = UIFactory.Panel(canvas, "LogoutRoot", new Color(0f, 0f, 0f, 0.45f));
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-20, -20);
            rt.sizeDelta = new Vector2(340, 48);
            UIFactory.Horizontal(root, 8, 8);

            var nameLabel = UIFactory.Text(root.transform, "", 22, TextAlignmentOptions.Right);
            nameLabel.color = Color.white;
            nameLabel.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;

            var logoutBtn = UIFactory.Button(root.transform, "Logout", null, new Color(0.85f, 0.5f, 0.5f));
            UIFactory.SetWidth(logoutBtn.gameObject, 140);

            var so = new SerializedObject(session);
            so.FindProperty("logoutRoot").objectReferenceValue = root;
            so.FindProperty("logoutButton").objectReferenceValue = logoutBtn;
            so.FindProperty("usernameLabel").objectReferenceValue = nameLabel;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildHud(Transform canvas, SessionUI hud)
        {
            var root = new GameObject("GameHud", typeof(RectTransform));
            root.transform.SetParent(canvas, false);
            UIFactory.FullScreen(root); // transparent, no raycast blocker

            // Bottom-left: Marketplace + Shop.
            var left = new GameObject("BottomLeft", typeof(RectTransform));
            left.transform.SetParent(root.transform, false);
            var lrt = left.GetComponent<RectTransform>();
            lrt.anchorMin = lrt.anchorMax = new Vector2(0f, 0f);
            lrt.pivot = new Vector2(0f, 0f);
            lrt.anchoredPosition = new Vector2(20f, 20f);
            UIFactory.Horizontal(left, 12, 0);
            var lfit = left.AddComponent<ContentSizeFitter>();
            lfit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            lfit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var marketBtn = UIFactory.Button(left.transform, "Marketplace", null, new Color(0.55f, 0.78f, 0.55f));
            UIFactory.SetWidth(marketBtn.gameObject, 190);
            UIFactory.SetHeight(marketBtn.gameObject, 58);
            var shopBtn = UIFactory.Button(left.transform, "Shop", null, new Color(0.9f, 0.75f, 0.4f));
            UIFactory.SetWidth(shopBtn.gameObject, 150);
            UIFactory.SetHeight(shopBtn.gameObject, 58);

            // Bottom-right: Inventory.
            var invBtn = UIFactory.Button(root.transform, "Inventory", null, new Color(0.55f, 0.7f, 0.9f));
            var irt = invBtn.GetComponent<RectTransform>();
            irt.anchorMin = irt.anchorMax = new Vector2(1f, 0f);
            irt.pivot = new Vector2(1f, 0f);
            irt.anchoredPosition = new Vector2(-20f, 20f);
            irt.sizeDelta = new Vector2(190f, 58f);

            var so = new SerializedObject(hud);
            so.FindProperty("hudRoot").objectReferenceValue = root;
            so.FindProperty("marketButton").objectReferenceValue = marketBtn;
            so.FindProperty("shopButton").objectReferenceValue = shopBtn;
            so.FindProperty("inventoryButton").objectReferenceValue = invBtn;
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
                // LoadingManager drives the transition into the game, so leave this empty.
                var bso = new SerializedObject(boot);
                bso.FindProperty("sceneToLoadAfterLogin").stringValue = "";
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
