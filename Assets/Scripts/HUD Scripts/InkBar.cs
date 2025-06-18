using UnityEngine;
using UnityEngine.UI;

namespace HUD_Scripts
{
    public class InkBar : MonoBehaviour
    {
        //To Fix
        private Slider _inkSlider;
        private PlayerInk _playerInk;
        private Image _fillImage;

        private void Awake()
        {
            PlayerInk.OnInkChange += UpdateInkUI;
            InkSelector.OnInkTypeChanged += UpdateInkColor;
            _inkSlider = GetComponent<Slider>();
            _fillImage = _inkSlider.fillRect.GetComponent<Image>();
            _playerInk = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInk>();
        }

        private void Start()
        {
            _inkSlider.maxValue = _playerInk.MaxInk;
            UpdateInkUI();
        }

        private void OnDestroy()
        {
            PlayerInk.OnInkChange -= UpdateInkUI;
            InkSelector.OnInkTypeChanged -= UpdateInkColor;
        }

        private void UpdateInkUI()
        {
            _inkSlider.value = _playerInk.CurrentInk;
        }

        private void UpdateInkColor(ElementType elementType)
        {
            Color newColor = GetColorForElementType(elementType);
            _fillImage.color = newColor;
        }

        private Color GetColorForElementType(ElementType elementType)
        {
            return elementType switch
            {
                ElementType.Fire => new Color(1f, 0.3f, 0.3f), // Red
                ElementType.Water => new Color(0.3f, 0.7f, 1f), // Blue
                ElementType.Electric => new Color(1f, 0.92f, 0.016f), // Yellow
                _ => Color.white
            };
        }
    }
}