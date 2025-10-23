using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UIModule
{
    public class CharacterSkinColorSelectView : MonoBehaviour
    {
        [SerializeField] private Toggle _colorToggle;
        [SerializeField] private Image _toggleImage;
        private string _toggleColorName;
        private string _toggleColorValue;

        public void Initialize(int togglecolorIndex,string toggleColorName, string toggleColor)
        {
            _toggleColorName = toggleColorName;
            _toggleImage.color = GetColorFromHex(toggleColor);
        }

        public Color GetColorFromHex(string toggleColor)
        {
            if (UnityEngine.ColorUtility.TryParseHtmlString(toggleColor, out Color color))
            {
                return color;
            }
            else
            {
                Debug.LogError("Invalid Hex color: " + toggleColor);
                return Color.white;
            }
        }
    }
}