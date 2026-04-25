using UnityEngine;

public class Portal : PlayerDetect
{
    [SerializeField] private string sceneToLoad;

    private void OnEnable()
    {
        onPlayerEnter += LoadScene;
    }

    private void OnDisable()
    {
        onPlayerEnter -= LoadScene;
    }

    private void LoadScene()
    {
        LoadingManager.Instance.LoadScene(sceneToLoad);
    }
}
