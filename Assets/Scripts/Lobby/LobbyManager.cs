using BattleRoyale.Network;
using BattleRoyale.Scene;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Multiplayer;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
  public static LobbyManager Instance { get; private set; }

    private Lobby _joinedLobby;

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
            options.SetProfile(Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, int lobbySize, bool isPrivate)
    {
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
        }
    }

    public async void QuickJoin()
    {
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
