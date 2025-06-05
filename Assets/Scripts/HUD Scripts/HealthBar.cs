using UnityEngine;
using UnityEngine.UI;

namespace HUD_Scripts
{
   public class HealthBar : MonoBehaviour
   {
      private PlayerHealth _playerHealth;
      private Slider _healthSlider;
      private void Awake()
      {
         PlayerHealth.OnTakeDamage += UpdateHealthUI;
         _healthSlider = GetComponent<Slider>();
         _playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
      }

      private void Start()
      {
         _healthSlider.maxValue = _playerHealth.MaxHealth;
         UpdateHealthUI();
      }

      private void OnDestroy()
      {
         PlayerHealth.OnTakeDamage -= UpdateHealthUI;
      }

      private void UpdateHealthUI()
      {
         _healthSlider.value = _playerHealth.CurrentHealth;
      }
   
   }
}
