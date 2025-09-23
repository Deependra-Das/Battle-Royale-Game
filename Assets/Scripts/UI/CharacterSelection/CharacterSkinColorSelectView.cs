using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class CharacterSkinColorSelectView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;
        private string _buttonColorName;
        private string _buttonColorValue;

        public void Initialize(string buttonColorName, string buttonColor)
        {
            _buttonColorName = buttonColorName;
            _buttonImage.color = GetColorFromHex(buttonColor);
            _button.onClick.AddListener(ChangeBackgroundColor);
        }

        public Color GetColorFromHex(string buttonColor)
        {
            if (UnityEngine.ColorUtility.TryParseHtmlString(buttonColor, out Color color))
            {
                return color;
            }
            else
            {
                Debug.LogError("Invalid Hex color: " + buttonColor);
                return Color.white;
            }
        }

        void ChangeBackgroundColor()
        {

        }
    }
}