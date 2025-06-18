using UnityEngine;
using UnityEngine.UI;

namespace HUD_Scripts
{
   public class HealthBar : MonoBehaviour
   {
      private PlayerHealth _playerHealth;
      private Slider _healthSlider;
      public void Initialize()
      {
         _healthSlider = GetComponent<Slider>();
         _playerHealth = GameManager.Instance.playerObject.GetComponent<PlayerHealth>();
         _healthSlider.maxValue = _playerHealth.MaxHealth;
         UpdateHealthUI();
         
         PlayerHealth.OnTakeDamage += UpdateHealthUI;
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
