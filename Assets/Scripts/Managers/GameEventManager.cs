using System;
using UnityEngine;

public class GameEventManager : Singleton<GameEventManager>
{
    public event Action OnToyInteract;

    public void TriggerToyInteract()
    {
        OnToyInteract?.Invoke();
    }
}
