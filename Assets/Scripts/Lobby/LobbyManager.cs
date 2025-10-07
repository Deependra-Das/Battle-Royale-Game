using BattleRoyale.Event;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
  public static LobbyManager Instance { get; private set; }

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
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

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.Instance.CURRENT_LOBBY_SIZE - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelateJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }


    public async void CreateLobby(string lobbyName, int lobbySize, bool isPrivate)
    {
        EventBusManager.Instance.RaiseNoParams(EventName.CreateLobbyStarted);
        try 
        {
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbySize,
            new CreateLobbyOptions { IsPrivate = isPrivate, });

            MultiplayerManager.Instance.SetCurrentLobbySize(lobbySize);

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelateJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

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

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

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

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

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

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

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
