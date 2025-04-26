using UnityEngine;
using BattleRoyale.Utilities;
using System.Collections;

public class GameManager : GenericMonoSingleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        InitializeServices();
    }

    private void InitializeServices()
    {
    }

    private void OnDestroy()
    {
    }
}
