using UnityEngine;

namespace BattleRoyale.UI
{
    [CreateAssetMenu(fileName = "UIScriptableObject", menuName = "ScriptableObjects/UIScriptableObject")]
    public class UIScriptableObject : ScriptableObject
    {
        public StartMenuUIView startMenuUIPrefab;
        public LobbyUIView lobbyUIPrefab;
        public GameplayUIView gameplayUIPrefab;
        public GameOverUIView gameOverUIPrefab;
    }
}
