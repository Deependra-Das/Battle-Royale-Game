using UnityEngine;
using TMPro;

namespace BattleRoyale.UIModule
{
    public class ScoreboardEntryUIView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _nameText;

        [SerializeField] private Sprite defaultAvatarSprite;

        public void SetupEntry(int rank, string playerName)
        {
            _rankText.text = rank.ToString();
            _nameText.text = playerName;
        }
    }
}