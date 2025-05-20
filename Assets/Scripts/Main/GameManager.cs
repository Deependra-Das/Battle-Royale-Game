using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using BattleRoyale.Utilities;
using BattleRoyale.Level;
using BattleRoyale.Player;
using BattleRoyale.Event;
using BattleRoyale.UI;
using BattleRoyale.Scene;
using Unity.Netcode;

namespace BattleRoyale.Main
{
    public class GameManager : GenericMonoSingleton<GameManager>
    {
        [SerializeField] public LevelScriptableObject level_SO;
        [SerializeField] public PlayerScriptableObject player_SO;
        [SerializeField] public UIScriptableObject ui_SO;

        private GameStateMachine _stateMachine;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _stateMachine = new GameStateMachine();
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }

        public void ChangeGameState(GameState newState)
        {
            switch (newState)
            {
                case GameState.Start:
                    _stateMachine.ChangeGameState(new StartState());
                    break;

                case GameState.Lobby:
                    _stateMachine.ChangeGameState(new LobbyState());
                    break;

                case GameState.CharacterSelection:
                    _stateMachine.ChangeGameState(new CharacterSelectionState());
                    break;

                case GameState.Gameplay:
                    _stateMachine.ChangeGameState(new GameplayState());
                    break;

                case GameState.GameOver:
                    _stateMachine.ChangeGameState(new GameOverState());
                    break;
            }

            EventBusManager.Instance.Raise(EventName.ChangeGameState, _stateMachine.GetCurrentState());
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}
