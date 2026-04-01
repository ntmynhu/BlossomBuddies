using UnityEngine;

public class TestServer : MonoBehaviour
{
    private async void Start()
    {
        var player = await HttpClient.Get<PlayerDto>("http://localhost:5250/api/Player/500");
        Debug.Log(player.Id);
    }
}