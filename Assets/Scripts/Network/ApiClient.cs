using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace BlossomBuddies.Network
{
    /// <summary>
    /// Central HTTP client for talking to the BlossomBuddies server.
    /// Coroutine-based (UnityWebRequest), stores the JWT, and attaches it as a Bearer token.
    /// Put this on a bootstrap GameObject that survives scene loads.
    /// </summary>
    public class ApiClient : MonoBehaviour
    {
        public static ApiClient Instance { get; private set; }

        [Header("Server")]
        [Tooltip("Base URL of the server. Editor/PC: https://localhost:7038. " +
                 "For a phone on the same Wi-Fi use your PC's LAN IP, e.g. http://192.168.1.10:5250")]
        [SerializeField] private string baseUrl = "https://localhost:7038";

        [Tooltip("DEV ONLY: accept the server's self-signed HTTPS certificate. " +
                 "Turn OFF for production / real certificates.")]
        [SerializeField] private bool bypassCertificateValidation = true;

        [Tooltip("Request timeout in seconds.")]
        [SerializeField] private int timeoutSeconds = 15;

        /// <summary>The current JWT, or null when not logged in.</summary>
        public string Token { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetToken(string token) => Token = token;
        public void ClearToken() => Token = null;

        public Coroutine Get<TRes>(string path, Action<TRes> onSuccess, Action<ApiError> onError)
            => StartCoroutine(SendRoutine("GET", path, null, onSuccess, onError));

        public Coroutine Post<TRes>(string path, object body, Action<TRes> onSuccess, Action<ApiError> onError)
            => StartCoroutine(SendRoutine("POST", path, body, onSuccess, onError));

        public Coroutine Delete<TRes>(string path, Action<TRes> onSuccess, Action<ApiError> onError)
            => StartCoroutine(SendRoutine("DELETE", path, null, onSuccess, onError));

        private IEnumerator SendRoutine<TRes>(string method, string path, object body,
            Action<TRes> onSuccess, Action<ApiError> onError)
        {
            var url = baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');

            using var request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = timeoutSeconds;

            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                var bytes = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bytes);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            request.SetRequestHeader("Accept", "application/json");
            if (IsLoggedIn)
                request.SetRequestHeader("Authorization", "Bearer " + Token);

            if (bypassCertificateValidation)
                request.certificateHandler = new AcceptAllCertificates();

            yield return request.SendWebRequest();

            var status = (int)request.responseCode;
            var text = request.downloadHandler != null ? request.downloadHandler.text : null;

            if (request.result != UnityWebRequest.Result.Success)
            {
                // Prefer the server's message body; fall back to the transport error.
                var message = !string.IsNullOrEmpty(text) ? text : request.error;
                onError?.Invoke(new ApiError(status, message));
                yield break;
            }

            try
            {
                var result = ParseResponse<TRes>(text);
                onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(new ApiError(status, "Failed to parse response: " + ex.Message));
            }
        }

        private static TRes ParseResponse<TRes>(string text)
        {
            // Endpoints that return a bare string (e.g. cancel) or empty body.
            if (typeof(TRes) == typeof(string))
                return (TRes)(object)(text ?? string.Empty);

            if (string.IsNullOrWhiteSpace(text))
                return default;

            return JsonConvert.DeserializeObject<TRes>(text);
        }
    }

    /// <summary>DEV ONLY certificate handler that trusts any server certificate.</summary>
    public class AcceptAllCertificates : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }

    public class ApiError
    {
        public int StatusCode { get; }
        public string Message { get; }

        public ApiError(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public override string ToString() => $"[{StatusCode}] {Message}";
    }
}
