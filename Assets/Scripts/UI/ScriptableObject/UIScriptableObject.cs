using UnityEngine;

namespace BattleRoyale.UI
{
    [CreateAssetMenu(fileName = "UIScriptableObject", menuName = "ScriptableObjects/UIScriptableObject")]
    public class UIScriptableObject : ScriptableObject
    {
        public StartMenuUIView startMenuUIPrefab;
        public LobbyUIView lobbyUIPrefab;
        public CharacterSelectionUIView characterSelectionUIPrefab;
        public GameplayUIView gameplayUIPrefab;
        public GameOverUIView gameOverUIPrefab;
        public ScoreboardEntryUIView scoreboardEntryUIPrefab;
        public float gameplayCountdownDuration;
        public float gameOverCountdownDuration;
    }
}
