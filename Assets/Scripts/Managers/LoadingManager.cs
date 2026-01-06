using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager>
{
    [SerializeField] private string initialSceneName = "MainScene";

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

    private IEnumerator InitialLoadWithSlider(string sceneName)
    {
        const float MIN_SLIDER_TIME = 2f;

        loadingCanvas.SetActive(true);
        loadingBarObject.SetActive(true);
        loadingBar.value = 0f;

        DataPersistenceManager.Instance.InitAndLoadGame();

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
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

        yield return Fade(0f, 1f, fadeInTime);

        InitAllScene();
        loadingCanvas.SetActive(false);

        yield return Fade(1f, 0f, fadeOutTime);
    }

    public void LoadScene(string sceneName)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOnlyLoad(sceneName));
    }

    private IEnumerator FadeOnlyLoad(string sceneName)
    {
        loadingCanvas.SetActive(true);
        loadingBarObject.SetActive(false);

        yield return Fade(0f, 1f, fadeOutTime);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

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
