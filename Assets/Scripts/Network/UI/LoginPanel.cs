using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlossomBuddies.Network.UI
{
    /// <summary>
    /// Login/register screen driven by scene-authored UI. Assign the references in the
    /// inspector. Put this component on an always-active object (e.g. the Canvas); the
    /// visual is <see cref="panelRoot"/>, which is hidden once the player is signed in.
    /// </summary>
    public class LoginPanel : MonoBehaviour
    {
        [Header("Root to hide when logged in")]
        [SerializeField] private GameObject panelRoot;

        [Header("Inputs")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;

        [Header("Buttons")]
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;

        [Header("Feedback")]
        [SerializeField] private TMP_Text statusText;

        private void Awake()
        {
            if (loginButton != null) loginButton.onClick.AddListener(OnLogin);
            if (registerButton != null) registerButton.onClick.AddListener(OnRegister);
        }

        private void Start()
        {
            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.OnLoggedIn += HandleLoggedIn;
                SessionManager.Instance.OnLoggedOut += HandleLoggedOut;

                if (SessionManager.Instance.IsLoggedIn) SetVisible(false);
            }
        }

        private void OnDestroy()
        {
            if (loginButton != null) loginButton.onClick.RemoveListener(OnLogin);
            if (registerButton != null) registerButton.onClick.RemoveListener(OnRegister);
            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.OnLoggedIn -= HandleLoggedIn;
                SessionManager.Instance.OnLoggedOut -= HandleLoggedOut;
            }
        }

        private void OnLogin()
        {
            if (!Validate(out var u, out var p)) return;
            SetBusy(true, "Logging in...");
            SessionManager.Instance.Login(u, p,
                onSuccess: () => { },
                onError: err => SetBusy(false, "Login failed: " + err.Message));
        }

        private void OnRegister()
        {
            if (!Validate(out var u, out var p)) return;
            SetBusy(true, "Creating account...");
            SessionManager.Instance.Register(u, p,
                onSuccess: () => { },
                onError: err => SetBusy(false, "Register failed: " + err.Message));
        }

        private bool Validate(out string user, out string pass)
        {
            user = usernameInput != null ? usernameInput.text.Trim() : null;
            pass = passwordInput != null ? passwordInput.text : null;

            if (SessionManager.Instance == null) { SetStatus("SessionManager not found in scene."); return false; }
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                SetStatus("Please enter username and password.");
                return false;
            }
            return true;
        }

        private void HandleLoggedIn() { SetBusy(false, ""); SetVisible(false); }
        private void HandleLoggedOut() { SetVisible(true); SetStatus("Signed out."); }

        private void SetVisible(bool visible)
        {
            if (panelRoot != null) panelRoot.SetActive(visible);
        }

        private void SetBusy(bool busy, string message)
        {
            if (loginButton != null) loginButton.interactable = !busy;
            if (registerButton != null) registerButton.interactable = !busy;
            SetStatus(message);
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
        }
    }
}
