using System.Globalization;
using TMPro;
using UnityEngine;

namespace HUD_Scripts
{
    public class InkText : MonoBehaviour
    {
      //To Fix
        private PlayerInk _playerInk;
        private TextMeshProUGUI _inkText;

        private void Awake()
        {
            PlayerInk.OnInkChange += UpdateInkUI;
            _inkText = GetComponent<TextMeshProUGUI>();
            _playerInk = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInk>();
        }

        private void Start()
        {
            UpdateInkUI();
        }

        private void OnDestroy()
        {
            PlayerInk.OnInkChange -= UpdateInkUI;
        }

        private void UpdateInkUI()
        {
            _inkText.text = _playerInk.CurrentInk.ToString(CultureInfo.InvariantCulture);
        }
    }
}