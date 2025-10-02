using TMPro;
using UnityEngine;

public class UIDebug : Singleton<UIDebug>
{
    [SerializeField] private TextMeshProUGUI debugText;

    public void SetDebugText(string text)
    {
        if (debugText != null)
        {
            debugText.text = text;
        }
    }
}
