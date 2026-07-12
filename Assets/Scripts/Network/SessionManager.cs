using System;
using UnityEngine;

namespace BlossomBuddies.Network
{
    /// <summary>
    /// Owns the player's authenticated session: persists the JWT, restores it on launch
    /// (auto-login), and exposes login/register/logout. Put this on the same bootstrap
    /// GameObject as <see cref="ApiClient"/>.
    /// </summary>
    [RequireComponent(typeof(ApiClient))]
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }

        private const string TokenKey = "bb_auth_token";
        private const string UserKey = "bb_auth_username";

        public bool IsLoggedIn => ApiClient.Instance != null && ApiClient.Instance.IsLoggedIn;
        public string Username { get; private set; }

        /// <summary>Fired after a successful login/register (token is set).</summary>
        public event Action OnLoggedIn;
        /// <summary>Fired after logout or when a stored session is cleared.</summary>
        public event Action OnLoggedOut;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Restore a previous session so returning players skip the login screen.
            var savedToken = PlayerPrefs.GetString(TokenKey, null);
            if (!string.IsNullOrEmpty(savedToken))
            {
                Username = PlayerPrefs.GetString(UserKey, null);
                ApiClient.Instance.SetToken(savedToken);
                OnLoggedIn?.Invoke();
            }
        }

        public void Register(string username, string password,
            Action onSuccess, Action<ApiError> onError)
        {
            GameApi.Auth.Register(username, password,
                res => HandleAuthSuccess(username, res, onSuccess),
                onError);
        }

        public void Login(string username, string password,
            Action onSuccess, Action<ApiError> onError)
        {
            GameApi.Auth.Login(username, password,
                res => HandleAuthSuccess(username, res, onSuccess),
                onError);
        }

        public void Logout()
        {
            Username = null;
            ApiClient.Instance.ClearToken();
            PlayerPrefs.DeleteKey(TokenKey);
            PlayerPrefs.DeleteKey(UserKey);
            PlayerPrefs.Save();
            OnLoggedOut?.Invoke();
        }

        private void HandleAuthSuccess(string username, AuthResponse res, Action onSuccess)
        {
            // GameApi already stored the token on ApiClient; persist it for next launch.
            Username = username;
            PlayerPrefs.SetString(TokenKey, res.Token);
            PlayerPrefs.SetString(UserKey, username);
            PlayerPrefs.Save();

            OnLoggedIn?.Invoke();
            onSuccess?.Invoke();
        }
    }
}
