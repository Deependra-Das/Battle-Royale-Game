using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using BattleRoyale.Utilities;
using BattleRoyale.Level;
using BattleRoyale.Player;

namespace BattleRoyale.Main
{
    public class GameManager : GenericMonoSingleton<GameManager>
    {
        [SerializeField] public LevelScriptableObject _level_SO;
        [SerializeField] public PlayerScriptableObject _player_SO;

        private GameStateMachine _stateMachine;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _stateMachine = new GameStateMachine();
            _stateMachine.ChangeGameState(new GameplayState());
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}
