using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _mainMixer;
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize mixer values from saved preferences
            float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            
            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSfxVolume(sfxVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (_musicSource == null || musicClip == null)
        {
            Debug.LogWarning("MusicSource or AudioClip not available.");
            return;
        }

        if (_musicSource.clip == musicClip && _musicSource.isPlaying)
        {
            return; // Already playing
        }

        _musicSource.Stop();
        _musicSource.clip = musicClip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            _musicSource.Stop();
        }
    }

    public void PlaySfx(AudioClip sfxClip, float volumeScale = 1.0f)
    {
        if (_sfxSource == null || sfxClip == null)
        {
            Debug.LogWarning("SFXSource or AudioClip not available.");
            return;
        }

        _sfxSource.PlayOneShot(sfxClip, volumeScale);
    }

    // --- Methods to Control Mixer Volume ---
    // The volume here is a linear value between 0.0001f (silence) and 1f (maximum)
    // The mixer uses decibels (dB), hence the Log10 conversion

    public void SetMasterVolume(float linearVolume)
    {
        SetMixerVolume(MASTER_VOLUME_KEY, linearVolume);
    }

    public void SetMusicVolume(float linearVolume)
    {
        SetMixerVolume(MUSIC_VOLUME_KEY, linearVolume);
    }

    public void SetSfxVolume(float linearVolume)
    {
        SetMixerVolume(SFX_VOLUME_KEY, linearVolume);
    }

    private void SetMixerVolume(string parameterName, float linearVolume)
    {
        if (_mainMixer == null) return;

        // Ensure the linear volume is in the range [0.0001, 1]
        // Log10(0) is -infinity, which completely silences the audio, that's why we use 0.0001 as a minimum
        float clampedVolume = Mathf.Clamp(linearVolume, 0.0001f, 1f);
        float dbVolume = Mathf.Log10(clampedVolume) * 20;
        _mainMixer.SetFloat(parameterName, dbVolume);
    }
}