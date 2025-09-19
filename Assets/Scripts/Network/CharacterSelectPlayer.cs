using BattleRoyale.Event;
using BattleRoyale.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text _usernameText;
    private int _playerIndex = -1;

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();

    private void SubscribeToEvents()
    {
        
    }

    private void UnsubscribeToEvents()
    {

    }

    public void Initialize(int assignedClientIndex, string usernameText)
    {
        SetPlayerIndexForCharacter(assignedClientIndex);
        SetUsernameForCharacter(usernameText);
    }

    public void SetPlayerIndexForCharacter(int assignedClientIndex)
    {
        _playerIndex = assignedClientIndex;
    }

    public void SetUsernameForCharacter(string usernameText)
    {
        _usernameText.text = usernameText;
    }

    private void ShowCharacter() => this.gameObject.SetActive(true);

    private void HideCharacter() => this.gameObject.SetActive(false);
}
