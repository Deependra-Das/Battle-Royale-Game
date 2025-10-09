using BattleRoyale.LobbyModule;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UIModule
{
    public class LobbyEntryUIView : MonoBehaviour
    {
        [SerializeField] private Button _lobbyEntryButton;
        [SerializeField] private TMP_Text _lobbyName;
        [SerializeField] private TMP_Text _lobbyCapacity;

        private Lobby _lobby;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _lobbyEntryButton.onClick.AddListener(OnLobbyEntryButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _lobbyEntryButton.onClick.RemoveListener(OnLobbyEntryButtonClicked);
        }

        public void Initialize(Lobby lobby)
        {
            _lobby = lobby;
            SetLobbyName(lobby.Name);
            SetLobbyCapacity(lobby.MaxPlayers);
        }

        private void SetLobbyName(string lobbyName)
        {
            _lobbyName.text = lobbyName;
        }

        private void SetLobbyCapacity(int lobbyCapacity)
        {
            _lobbyCapacity.text = lobbyCapacity.ToString();
        }

        private void OnLobbyEntryButtonClicked()
        {
            LobbyManager.Instance.JoinWithId(_lobby.Id);
        }
    }
}