using UnityEngine;
using BattleRoyale.Utilities;
using System.Collections;
using BattleRoyale.Level;

public class GameManager : GenericMonoSingleton<GameManager>
{
    [SerializeField] private LevelScriptableObject _level_SO;

    protected override void Awake()
    {
        base.Awake();
        InitializeServices();
    }

    private void Start()
    {
        Get<LevelService>().StartLevel(); //For Testing
    }

    private void InitializeServices()
    {
        ServiceLocator.Register(new LevelService(_level_SO));
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<LevelService>();
    }

    public T Get<T>()
    {
        return ServiceLocator.Get<T>();
    }
}
