using UnityEngine;
using BattleRoyale.Utilities;
using System.Collections;
using BattleRoyale.Level;
using UnityEngine.SceneManagement;

namespace BattleRoyale.Main
{
    public class GameManager : GenericMonoSingleton<GameManager>
    {
        [SerializeField] public LevelScriptableObject _level_SO;
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
