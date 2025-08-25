using BattleRoyale.Event;
using BattleRoyale.Network;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    private int _playerIndex = -1;

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();

    private void SubscribeToEvents()
    {
        
    }

    private void UnsubscribeToEvents()
    {

    }

    public void Initialize(int assignedClientIndex)
    {
        _playerIndex = assignedClientIndex;
    }

    private void ShowCharacter() => this.gameObject.SetActive(true);

    private void HideCharacter() => this.gameObject.SetActive(false);
}
