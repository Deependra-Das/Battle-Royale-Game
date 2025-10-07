using BattleRoyale.Event;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
  public static LobbyManager Instance { get; private set; }

    private Lobby _joinedLobby;
    private float _heartbeatTimer = 0;
    private float _heartbeatTimerMax = 15f;
    private float _listLobbiesTimer = 0;
    private float _listLobbiesTimerMax = 3f;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicFetchLobbyList();
    }

    private void HandleHeartbeat()
    {
       if(isLobbyHost())
        {
            _heartbeatTimer -= Time.deltaTime;

            if(_heartbeatTimer <= 0f)
            {
                _heartbeatTimer = _heartbeatTimerMax;
                LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }
    }

    private void HandlePeriodicFetchLobbyList()
    {
        string activeSceneName = SceneManager.GetActiveScene().name.ToString();
        Enum.TryParse<SceneName>(activeSceneName, out var sceneEnumValue);

        if (_joinedLobby == null && AuthenticationService.Instance.IsSignedIn && sceneEnumValue != SceneName.CharacterSelectionScene)
        {
            _listLobbiesTimer -= Time.deltaTime;
            if (_listLobbiesTimer <= 0f)
            {
                _listLobbiesTimer = _listLobbiesTimerMax;
                FetchLobbyList();
            }
        }
    }

    private bool isLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void CreateLobby(string lobbyName, int lobbySize, bool isPrivate)
    {
        EventBusManager.Instance.RaiseNoParams(EventName.CreateLobbyStarted);
        try 
        { 
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbySize,
            new CreateLobbyOptions { IsPrivate = isPrivate, });

            Debug.Log($"Lobby created: {_joinedLobby.Name} (ID: {_joinedLobby.Id})");

            MultiplayerManager.Instance.SetCurrentLobbySize(lobbySize);
            MultiplayerManager.Instance.StartHost();
            SceneLoader.Instance.LoadScene(SceneName.CharacterSelectionScene, true);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            EventBusManager.Instance.RaiseNoParams(EventName.CreateLobbyFailed);
        }
    }

    public async void QuickJoin()
    {
        EventBusManager.Instance.RaiseNoParams(EventName.JoinStarted);
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            EventBusManager.Instance.RaiseNoParams(EventName.QuickJoinFailed);
        }
    }
    public async void JoinWithId(string lobbyId)
    {
        EventBusManager.Instance.RaiseNoParams(EventName.JoinStarted);
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            EventBusManager.Instance.RaiseNoParams(EventName.JoinFailed);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        EventBusManager.Instance.RaiseNoParams(EventName.JoinStarted);
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            EventBusManager.Instance.RaiseNoParams(EventName.JoinFailed);
        }
    }

    public async void DeleteLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);

                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public Lobby GetLobby()
    {
        if(_joinedLobby!=null)
        {
            return _joinedLobby;
        }

        return null;
    }

    private async void FetchLobbyList()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter( QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            EventBusManager.Instance.Raise(EventName.PublicLobbyListChanged, queryResponse.Results);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
