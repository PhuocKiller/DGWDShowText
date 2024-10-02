using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkCallBack : MonoBehaviour,INetworkRunnerCallbacks
{
    Action<List<SessionInfo>> onSessionListChanged;
    Action<NetworkRunner, PlayerRef> onPlayerJoin;
    public void StartGameRegister(Action<List<SessionInfo>> onSessionListChanged)
    {
        this.onSessionListChanged = onSessionListChanged;
    }
    public void OnPlayerJoinRegister(Action<NetworkRunner, PlayerRef> onPlayerJoin)
    {
        this.onPlayerJoin = onPlayerJoin;
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        onSessionListChanged?.Invoke(sessionList);
    }
    #region networkCallBack
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        onPlayerJoin?.Invoke(runner, player);
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
       
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
       
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
       
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
       
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    //server or host
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
       
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
       
    }

   

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
       
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
       
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
       
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
       
    }

  

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
       
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       
    }
    #endregion
    void Start()
    {
        
    }

   
}
