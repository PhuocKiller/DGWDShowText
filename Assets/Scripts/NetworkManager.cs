using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviour
{
    NetworkRunner runner;
    [SerializeField]
    private UnityEvent onServerStart;
    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
    }
    public async void StartGame()
    {
        await runner.StartGame(new StartGameArgs { GameMode = GameMode.Shared,
        SessionName = "Hello Photon"
        });
        onServerStart?.Invoke();
    }
}
