using UnityEngine;

namespace BattleRoyale.PlayerModule
{
    [CreateAssetMenu(fileName = "PlayerCharMatSkinColorScriptableObject", menuName = "ScriptableObjects/PlayerCharMatSkinColorScriptableObject")]
    public class PlayerCharMatSkinColorScriptableObject : ScriptableObject
    {
        public PlayerCharacterSkinMaterialInfo[] charSkinInfoList;
    }
}
