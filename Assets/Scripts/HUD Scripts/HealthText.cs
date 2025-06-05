using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD_Scripts
{
   public class HealthText : MonoBehaviour
   {
      private PlayerHealth _playerHealth;
      private TextMeshProUGUI _healthText;
      private void Awake()
      {
         PlayerHealth.OnTakeDamage += UpdateHealthUI;
         _healthText = GetComponent<TextMeshProUGUI>();
         _playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

      }

      private void Start()
      {
         UpdateHealthUI();
      }

      private void OnDestroy()
      {
         PlayerHealth.OnTakeDamage -= UpdateHealthUI;
      }

      private void UpdateHealthUI()
      {
         _healthText.text = _playerHealth.CurrentHealth.ToString(CultureInfo.InvariantCulture);
      }
   
   }
}
