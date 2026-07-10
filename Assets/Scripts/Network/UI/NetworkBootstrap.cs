using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// Wires the online layer. Put this on an object in the entry (Login) scene together with
    /// ApiClient / SessionManager / ServerSyncManager. The UI is now authored directly in the
    /// scene (a Canvas with LoginPanel + MarketPanel), so this no longer builds UI at runtime.
    ///
    /// - Keeps the referenced <see cref="persistentCanvas"/> alive across scene loads so the
    ///   market UI survives into the game scene.
    /// - Loads <see cref="sceneToLoadAfterLogin"/> once the player signs in (or a saved session
    ///   is restored). Leave empty if this already lives in the game scene.
    /// </summary>
    public class NetworkBootstrap : MonoBehaviour
    {
        public static NetworkBootstrap Instance { get; private set; }

        [Tooltip("Scene to load after a successful login / restored session. Leave empty if already in the game scene.")]
        [SerializeField] private string sceneToLoadAfterLogin = "";

        [Tooltip("UI canvas kept alive across scene loads (login + market).")]
        [SerializeField] private Canvas persistentCanvas;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Fallbacks in case the components weren't added in the inspector.
            if (GetComponent<ApiClient>() == null) gameObject.AddComponent<ApiClient>();
            if (GetComponent<SessionManager>() == null) gameObject.AddComponent<SessionManager>();
            if (GetComponent<ServerSyncManager>() == null) gameObject.AddComponent<ServerSyncManager>();

            if (persistentCanvas != null)
                DontDestroyOnLoad(persistentCanvas.gameObject);

            if (SessionManager.Instance != null)
                SessionManager.Instance.OnLoggedIn += HandleLoggedIn;
        }

        private void OnDestroy()
        {
            if (Instance == this && SessionManager.Instance != null)
                SessionManager.Instance.OnLoggedIn -= HandleLoggedIn;
        }

        private void HandleLoggedIn()
        {
            if (string.IsNullOrEmpty(sceneToLoadAfterLogin)) return;
            if (SceneManager.GetActiveScene().name == sceneToLoadAfterLogin) return;

            SceneManager.LoadScene(sceneToLoadAfterLogin);
        }
    }
}
