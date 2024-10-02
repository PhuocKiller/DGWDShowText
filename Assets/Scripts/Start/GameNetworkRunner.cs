using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameNetworkRunner : MonoBehaviour
{
    [SerializeField]
    private GameObject roomItem;
    [SerializeField]
    private Transform parentRoomItem;
    private NetworkRunner runner;
    private GameNetworkCallBack gameNetworkCallBack;
    private string roomName = "";

    [SerializeField]
    private UnityEvent onConnected;
    [SerializeField]
    private GameObject roboPlayer;
    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private GameObject readyText;
    [SerializeField]
    private GameObject playerManagerObj;

    [SerializeField]
    private GameObject gameManagerObj;
    private void SpawnPlayer(NetworkRunner m_runner, PlayerRef player)
    {
        if(player == runner.LocalPlayer && runner.IsSharedModeMasterClient)
        {
            runner.Spawn(gameManagerObj, inputAuthority: player);
            runner.Spawn(playerManagerObj, inputAuthority: player);
        }

        if (player == runner.LocalPlayer && spawnPoints.Length > player.PlayerId )
        {
           NetworkObject robo = runner.Spawn(roboPlayer, spawnPoints[player.PlayerId].position,Quaternion.identity,inputAuthority: player
               ,onBeforeSpawned: OnBeforeSpawned);

            void OnBeforeSpawned(NetworkRunner runner, NetworkObject roboObject)
            {
                NetworkObject textR = runner.Spawn(readyText, roboObject.transform.position, inputAuthority: player);
                TrackingReady trackingReady = textR.GetComponent<TrackingReady>();
                trackingReady.SetRoboTrans(roboObject.transform);
                roboObject.GetComponent<RoboController>().SetSpawnPoint(spawnPoints[player.PlayerId].position);
                roboObject.GetComponent<RoboController>().SetTrackingReady(trackingReady);
            }

        }
        else
        {
            Debug.Log("No Spawn: "+ player.PlayerId);
        }
       
    }
  
    public void TextChanged(string text)
    {
        roomName = text.Trim();
    }
    public async void OnClickBtn(Button btn)
    {
        if (roomName.Length > 0 && runner != null)
        {
            btn.interactable = false;
            Singleton<Loading>.Instance.ShowLoading();
            gameNetworkCallBack ??= GetComponent<GameNetworkCallBack>();
            gameNetworkCallBack.OnPlayerJoinRegister(SpawnPlayer);
            await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomName,
                CustomLobbyName = "VN",
                SceneManager = GetComponent<LoadSceneManager>()
            });
            onConnected?.Invoke();
            Singleton<Loading>.Instance.HideLoading();
        }
    }
    public async void OnClickJoinBtn()
    {
        Singleton<Loading>.Instance.ShowLoading();
        gameNetworkCallBack ??= GetComponent<GameNetworkCallBack>();
        gameNetworkCallBack.StartGameRegister(OnSessionListChanged);
        await runner.JoinSessionLobby(SessionLobby.Custom, "VN");
        Singleton<Loading>.Instance.HideLoading();
    }
    private void OnSessionListChanged(List<SessionInfo> sessionInfos)
    {
        foreach (Transform child in parentRoomItem)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in sessionInfos)
        {
            GameObject room = Instantiate(roomItem, parentRoomItem);
            room.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
            Button btn = room.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                roomName = item.Name;
                OnClickBtn(btn);
            });
        }
    }
    private void Awake()
    {
        runner ??= GetComponent<NetworkRunner>();
        gameNetworkCallBack ??= GetComponent<GameNetworkCallBack>();
    }
}
