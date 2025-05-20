using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameStateMachine
    {
        private IGameState _currentState;
        protected Dictionary<GameState, IGameState> _states = new Dictionary<GameState, IGameState>();

        public IGameState CurrentState => _currentState;

        public GameStateMachine()
        {
            CreateStates();
        }

        private void CreateStates()
        {
            _states.Add(GameState.Start, new StartState());
            _states.Add(GameState.Lobby, new LobbyState());
            _states.Add(GameState.CharacterSelection, new CharacterSelectionState());
            _states.Add(GameState.Gameplay, new GameplayState());
            _states.Add(GameState.GameOver, new GameOverState());
        }

        public void ChangeGameState(IGameState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public GameState GetCurrentState()
        {
            foreach (var state in _states)
            {
                if (state.Value == _currentState)
                    return state.Key;
            }
            return GameState.Start;
        }
    }
}
