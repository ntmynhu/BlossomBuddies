using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// Shows the signed-in username and a logout control (visible only while signed in).
    /// Logging out saves progress, clears the session, and re-runs the load flow so the
    /// login screen shows again and a new account can enter the game (test multiple accounts).
    /// </summary>
    public class SessionUI : MonoBehaviour
    {
        [SerializeField] private GameObject logoutRoot;   // shown only when logged in
        [SerializeField] private Button logoutButton;
        [SerializeField] private TMP_Text usernameLabel;

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
            // Push the current account's progress to the cloud before switching (token still valid).
            if (DataPersistenceManager.Instance != null) DataPersistenceManager.Instance.SaveGame();

            if (SessionManager.Instance != null) SessionManager.Instance.Logout();

            // LoadingManager is persistent; restart its flow so the next login re-enters the game.
            if (LoadingManager.Instance != null) LoadingManager.Instance.RestartLoadFlow();
        }

        private void Show() => SetVisible(true);
        private void Hide() => SetVisible(false);

        private void SetVisible(bool visible)
        {
            if (logoutRoot != null) logoutRoot.SetActive(visible);

            if (visible && usernameLabel != null && SessionManager.Instance != null)
                usernameLabel.text = SessionManager.Instance.Username;
        }
    }
}
