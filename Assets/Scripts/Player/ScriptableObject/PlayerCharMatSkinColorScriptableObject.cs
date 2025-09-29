using UnityEngine;

namespace BattleRoyale.Player
{
    [CreateAssetMenu(fileName = "PlayerCharMatSkinColorScriptableObject", menuName = "ScriptableObjects/PlayerCharMatSkinColorScriptableObject")]
    public class PlayerCharMatSkinColorScriptableObject : ScriptableObject
    {
        public PlayerCharacterSkinMaterialInfo[] charSkinInfoList;
    }
}
