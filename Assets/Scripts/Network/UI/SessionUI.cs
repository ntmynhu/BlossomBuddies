using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// Small logout control, visible only while signed in. Logging out clears the session
    /// and reloads the entry scene so the login gate shows again (handy for testing accounts).
    /// </summary>
    public class SessionUI : MonoBehaviour
    {
        [SerializeField] private GameObject logoutRoot;   // shown only when logged in
        [SerializeField] private Button logoutButton;
        [SerializeField] private string sceneAfterLogout = "LoadingScene";

        private void Awake()
        {
            if (logoutButton != null) logoutButton.onClick.AddListener(OnLogout);
        }

        private void Start()
        {
            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.OnLoggedIn += Show;
                SessionManager.Instance.OnLoggedOut += Hide;
                SetVisible(SessionManager.Instance.IsLoggedIn);
            }
            else
            {
                SetVisible(false);
            }
        }

        private void OnDestroy()
        {
            if (logoutButton != null) logoutButton.onClick.RemoveListener(OnLogout);
            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.OnLoggedIn -= Show;
                SessionManager.Instance.OnLoggedOut -= Hide;
            }
        }

        private void OnLogout()
        {
            if (SessionManager.Instance != null) SessionManager.Instance.Logout();
            if (!string.IsNullOrEmpty(sceneAfterLogout))
                SceneManager.LoadScene(sceneAfterLogout);
        }

        private void Show() => SetVisible(true);
        private void Hide() => SetVisible(false);

        private void SetVisible(bool visible)
        {
            if (logoutRoot != null) logoutRoot.SetActive(visible);
        }
    }
}
