using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Unity.Cinemachine;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    NetworkPlayer networkPlayer;

    CharacterInputManager characterInputManager;
    [SerializeField] private CinemachineCamera playerCameraPrefab;

    private CinemachineCamera _playerCamera;

    void Start()
    {
        
    }

    public void SetPlayerCamera(Vector3 camIntialPosition)
    {
        _playerCamera = Instantiate(playerCameraPrefab, camIntialPosition, Quaternion.identity);
        CharacterMovementController charMoveCon = networkPlayer.gameObject.GetComponent<CharacterMovementController>();
        _playerCamera.Follow = charMoveCon.cinemachineCameraTarget.transform;
    }

    public void OnConnectedToServer(NetworkRunner runner) 
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
      if(runner.IsServer)
        {
            Debug.Log("OnPlayerJoined | We are Server. Spawning Player.");
            networkPlayer = runner.Spawn(playerPrefab, transform.position, Quaternion.identity, player);

            SetPlayerCamera(transform.position);
        }
        else
        {
            Debug.Log("OnPlayerJoined");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputManager == null && NetworkPlayer.Local != null)
        {
            characterInputManager = NetworkPlayer.Local.GetComponent<CharacterInputManager>();
        }

        if (characterInputManager != null)
        {
            input.Set(characterInputManager.GetNetworkInput());
        }
    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnectRequest");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }















}
