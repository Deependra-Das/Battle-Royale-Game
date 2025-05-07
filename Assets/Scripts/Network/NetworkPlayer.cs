using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using static Fusion.NetworkBehaviour;
using Unity.Cinemachine;
using static Unity.Collections.Unicode;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TextMeshProUGUI playerNameText;
    [SerializeField] private CinemachineCamera playerCameraPrefab;
    [SerializeField] public Transform cinemachineCameraTarget;

    CinemachineCamera localPlayerCamera;
    public static NetworkPlayer Local { get; set; }

    [Networked]
    public NetworkString<_16> playerName { get; set; }

    private ChangeDetector _changeDetector;

    void Start()
    {
        
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (Object.HasInputAuthority)
        {
            Local = this;

            RPC_SetPlayerName(PlayerPrefs.GetString("PlayerName")); ;

            Debug.Log("Spawned local player");

            InitializePlayerCamera();
            OnAddPlayerCameraEvent?.Invoke(this);
        }
        else
        {
            Debug.Log("Spawned remote player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(playerName):
                    Debug.Log(PlayerPrefs.GetString("PlayerName"));
                    OnNameChanged();
                    break;
            }
        }
    }

    private void OnNameChanged()
    {
        Debug.Log(playerName);
        playerNameText.text = playerName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerName(string playerName, RpcInfo info = default)
    {
        this.playerName = playerName;
    }

    public void InitializePlayerCamera()
    {
       localPlayerCamera = Instantiate(playerCameraPrefab, transform.position, Quaternion.identity);
       localPlayerCamera.Follow = cinemachineCameraTarget;
    }

    public static event System.Action<NetworkPlayer> OnAddPlayerCameraEvent;
}
