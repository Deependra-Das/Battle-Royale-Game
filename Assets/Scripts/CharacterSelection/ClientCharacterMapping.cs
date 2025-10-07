using UnityEngine;

namespace BattleRoyale.CharacterSelectionModule
{
    public class ClientCharacterMapping
    {
        public ulong clientID;
        public GameObject character;

        public ClientCharacterMapping(ulong clientID, GameObject character)
        {
            this.clientID = clientID;
            this.character = character;
        }
    }
}
