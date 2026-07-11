using System.Collections;
using System.Collections.Generic;
using BlossomBuddies.Network;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager>
{
    [SerializeField] private SceneName initialSceneName = SceneName.MainScene;

    [Header("UI")]
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject loadingBarObject;
    [SerializeField] private Slider loadingBar;

    [Header("Fade Overlay")]
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("Timing")]
    [SerializeField] private float fadeOutTime = 0.3f;
    [SerializeField] private float fadeInTime = 0.3f;

    private static bool hasShownInitialLoading = false;

    protected override void Awake()
    {
        base.Awake();
    
        if (fadeCanvasGroup == null)
            fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();

        SetFadeInstant(0f);
        loadingCanvas.SetActive(false);
    }

    private IEnumerator Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.loadingMusicClip);

        // Login gate: don't enter the game until authenticated. The LoginPanel overlay
        // (on the persistent OnlineCanvas) handles sign-in; a restored session passes instantly.
        yield return new WaitUntil(() => SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn);

        if (!hasShownInitialLoading)
        {
            yield return InitialLoadWithSlider(initialSceneName);
            hasShownInitialLoading = true;
        }
        else
        {
            yield return FadeOnlyLoad(initialSceneName);
        }
    }

    private IEnumerator InitialLoadWithSlider(SceneName sceneName)
    {
        const float MIN_SLIDER_TIME = 2f;

        loadingCanvas.SetActive(true);
        loadingBarObject.SetActive(true);
        loadingBar.value = 0f;

        // Load this account's progress from the server cloud save (defaults for new accounts).
        bool saveReady = false;
        DataPersistenceManager.Instance.LoadFromServer(() => saveReady = true);
        yield return new WaitUntil(() => saveReady);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName.ToString());
        op.allowSceneActivation = false;

        float elapsed = 0f;
        float visualProgress = 0f;

        // Đợi cả: load xong + đủ thời gian
        while (op.progress < 0.9f || elapsed < MIN_SLIDER_TIME)
        {
            elapsed += Time.deltaTime;

            // progress thật của scene (0..1)
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);

            // slider hiển thị tiến dần đều, không bị giật
            visualProgress = Mathf.MoveTowards(
                visualProgress,
                realProgress,
                Time.deltaTime * 0.5f
            );

            loadingBar.value = visualProgress;
            yield return null;
        }

        loadingBar.value = 1f;

        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        GameManager.Instance.SetCurrentScene(sceneName);

        yield return Fade(0f, 1f, fadeInTime);
        yield return new WaitForSeconds(0.5f);

        InitAllScene();

        // Inventory + coins are authoritative in their own server tables (so the marketplace
        // can read them). Pull them now to override whatever the cloud-save blob contained.
        if (ServerSyncManager.Instance != null)
            ServerSyncManager.Instance.PullFromServer();

        loadingCanvas.SetActive(false);

        yield return Fade(1f, 0f, fadeOutTime);
    }

    public void LoadScene(SceneName sceneName)
    {
        DataPersistenceManager.Instance.SaveGame();

        StopAllCoroutines();
        StartCoroutine(FadeOnlyLoad(sceneName));
    }

    private IEnumerator FadeOnlyLoad(SceneName sceneName)
    {
        loadingCanvas.SetActive(true);
        loadingBarObject.SetActive(false);

        yield return Fade(0f, 1f, fadeOutTime);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName.ToString());
        while (!op.isDone) yield return null;

        GameManager.Instance.SetCurrentScene(sceneName);
        DataPersistenceManager.Instance.LoadGame();

        yield return Fade(1f, 0f, fadeInTime);

        loadingCanvas.SetActive(false);
    }

    public IEnumerator Fade(float from, float to, float duration)
    {
        fadePanel.SetActive(true);
        fadeCanvasGroup.blocksRaycasts = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;

        if (to == 0f)
        {
            fadeCanvasGroup.blocksRaycasts = false;
            fadePanel.SetActive(false);
        }
    }

    private void SetFadeInstant(float alpha)
    {
        fadeCanvasGroup.alpha = alpha;
        fadeCanvasGroup.blocksRaycasts = alpha > 0f;
        fadePanel.SetActive(alpha > 0f);
    }

    private void InitAllScene()
    {
        DataPersistenceManager.Instance.PushLoadedDataToObject();

        DataPersistenceManager.Instance.SetLoadedDataDone();
    }
}
