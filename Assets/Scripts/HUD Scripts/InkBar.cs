using UnityEngine;
using UnityEngine.UI;

namespace HUD_Scripts
{
    public class InkBar : MonoBehaviour
    {
        private Slider _inkSlider;
        private PlayerInk _playerInk;

        private void Awake()
        {
            PlayerInk.OnInkChange += UpdateInkUI;
            _inkSlider = GetComponent<Slider>();
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
        }

        private void UpdateInkUI()
        {
            _inkSlider.value = _playerInk.CurrentInk;
        }
    }
}