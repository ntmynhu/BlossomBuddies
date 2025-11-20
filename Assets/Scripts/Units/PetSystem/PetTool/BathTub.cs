using UnityEngine;

public class BathTub : PlayerDetect
{
    private void OnInteract()
    {
        if (isPlayerInRange)
        {
            Debug.Log("Interacting with BathTub");
        }
    }
}
