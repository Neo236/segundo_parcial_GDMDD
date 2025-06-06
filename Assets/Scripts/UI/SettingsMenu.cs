using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SettingsMenu : MonoBehaviour
{
    //[SerializeField] private PauseMenu pauseMenuScript;
    [Header("In-Game Settings UI")]
    [SerializeField] private GameObject inGameSettingsPanel;
    
    [SerializeField] private Slider inGameMasterVolumeSlider;
    [SerializeField] private Slider inGameMusicVolumeSlider;
    [SerializeField] private Slider inGameSfxVolumeSlider;

    [Header("Main Menu Settings UI")]
    [SerializeField] private GameObject mainMenuSettingsPanel;
    [SerializeField] private Slider mainMenuMasterVolumeSlider;
    [SerializeField] private Slider mainMenuMusicVolumeSlider;
    [SerializeField] private Slider mainMenuSfxVolumeSlider;
    public event Action OnBackAction;
    
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //InitializeSlider(inGameMasterVolumeSlider);
        //InitializeSlider(inGameMusicVolumeSlider);
        //InitializeSlider(inGameSfxVolumeSlider);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            var mainMenuCanvas = GameObject.Find("MainMenuCanvas");
            if (mainMenuCanvas != null)
            {
                mainMenuSettingsPanel = mainMenuCanvas.transform.Find("SettingsMenu").gameObject;
                mainMenuMasterVolumeSlider = mainMenuSettingsPanel.transform.Find("MasterVolume").GetComponent<Slider>();
                mainMenuMusicVolumeSlider = mainMenuSettingsPanel.transform.Find("MusicVolume").GetComponent<Slider>();
                mainMenuSfxVolumeSlider = mainMenuSettingsPanel.transform.Find("SFXVolume").GetComponent<Slider>();
                
                SetupSliders(mainMenuMasterVolumeSlider, mainMenuMusicVolumeSlider, mainMenuSfxVolumeSlider);
            }
        }
    }
    public void SetupInGameSliders()
    {
        SetupSliders(inGameMasterVolumeSlider, inGameMusicVolumeSlider, inGameSfxVolumeSlider);
    }
    
    private void SetupSliders(Slider master, Slider music, Slider sfx)
    {
        if (master == null || music == null || sfx == null)
        {
            Debug.LogWarning("Uno o más sliders no fueron encontrados durante la configuración.");
            return;
        }

        // Limpiamos listeners anteriores para evitar duplicados
        master.onValueChanged.RemoveAllListeners();
        music.onValueChanged.RemoveAllListeners();
        sfx.onValueChanged.RemoveAllListeners();
        
        // Inicializamos valores
        InitializeSlider(master);
        InitializeSlider(music);
        InitializeSlider(sfx);
        
        // Cargamos valores de PlayerPrefs
        master.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        // Añadimos los nuevos listeners
        master.onValueChanged.AddListener(OnMasterVolumeChanged);
        music.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfx.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    private void InitializeSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0.0001f;
            slider.maxValue = 1f;
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
        if (inGameSettingsPanel.activeSelf || 
            GameObject.Find("MainMenuCanvas")?.transform.Find("SettingsMenu")
                .gameObject.activeSelf == true)
        {
            OnBackButtonClicked();
        }
    }

    public void OnBackButtonClicked()
    {
        OnBackAction?.Invoke();
    }
}