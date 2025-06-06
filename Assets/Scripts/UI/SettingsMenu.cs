using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenuScript;
    [SerializeField] private GameObject settingsPanel;
    
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Awake()
    {
        InitializeSlider(masterVolumeSlider);
        InitializeSlider(musicVolumeSlider);
        InitializeSlider(sfxVolumeSlider);
    }

    private void InitializeSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0.0001f;
            slider.maxValue = 1f;
        }
    }

    private void Start()
    {
        LoadSliderValues();
    }
    
    private void LoadSliderValues()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }
    
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
    
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
    }

    public void OnMasterVolumeChanged(float value)
    {
        Debug.Log($"Master Volume Slider cambió. Nuevo valor recibido: {value}");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
            PlayerPrefs.SetFloat("MasterVolume", value);
            PlayerPrefs.Save();
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
            PlayerPrefs.SetFloat("MusicVolume", value);
            PlayerPrefs.Save();
        }
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSfxVolume(value);
            PlayerPrefs.SetFloat("SFXVolume", value);
            PlayerPrefs.Save();
        }
    }

    private void OnEnable()
    {
        MenuInput.OnBackButtonPressed += HandleBackButton;
    }

    private void OnDisable()
    {
        MenuInput.OnBackButtonPressed -= HandleBackButton;
    }

    private void HandleBackButton()
    {
        if (settingsPanel.activeSelf)
        {
            OnBackButtonClicked();
        }
    }

    public void OnBackButtonClicked()
    {
        settingsPanel.SetActive(false);
        if (pauseMenuScript != null)
        {
            pauseMenuScript.ReturnFromSettings();
        }
    }
}