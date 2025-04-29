using UnityEngine;

namespace BattleRoyale.UI
{
    [CreateAssetMenu(fileName = "UIScriptableObject", menuName = "ScriptableObjects/UIScriptableObject")]
    public class UIScriptableObject : ScriptableObject
    {
        public StartMenuUIView startMenuUIPrefab;
        public GameOverUIView gameOverUIPrefab;
    }
}
