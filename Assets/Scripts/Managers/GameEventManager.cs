using System;
using UnityEngine;

public class GameEventManager : Singleton<GameEventManager>
{
    public event Action OnToyInteract;
    public event Action OnHeartNumberChange;

    public void TriggerToyInteract()
    {
        OnToyInteract?.Invoke();
    }

    public void TriggerHeartNumberChange()
    {
        OnHeartNumberChange?.Invoke();
    }
}
