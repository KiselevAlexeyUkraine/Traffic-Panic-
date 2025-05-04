using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
using System;

public class FmodVcaVolumeController : MonoBehaviour
{
    [Header("VCA References")]
    [SerializeField] private string masterVcaPath = "vca:/Master";
    [SerializeField] private string sfxVcaPath = "vca:/SFX";
    [SerializeField] private string musicVcaPath = "vca:/Music";

    [Header("UI Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private FMOD.Studio.VCA masterVca;
    private FMOD.Studio.VCA sfxVca;
    private FMOD.Studio.VCA musicVca;

    private void Awake()
    {
        try
        {
            // Инициализация VCA
            masterVca = RuntimeManager.GetVCA(masterVcaPath);
            sfxVca = RuntimeManager.GetVCA(sfxVcaPath);
            musicVca = RuntimeManager.GetVCA(musicVcaPath);

            InitializeSliders();
        }
        catch (Exception e)
        {
            Debug.LogError($"FMOD VCA Initialization failed: {e.Message}");
        }
    }

    private void InitializeSliders()
    {
        // Устанавливаем начальные значения слайдеров
        SetSliderValue(masterVca, masterVolumeSlider, "MasterVolume");
        SetSliderValue(sfxVca, sfxVolumeSlider, "SFXVolume");
        SetSliderValue(musicVca, musicVolumeSlider, "MusicVolume");

        // Подписываемся на изменения
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void SetSliderValue(FMOD.Studio.VCA vca, Slider slider, string playerPrefKey)
    {
        if (vca.isValid())
        {
            vca.getVolume(out float volume);
            slider.value = PlayerPrefs.HasKey(playerPrefKey) ?
                PlayerPrefs.GetFloat(playerPrefKey) : volume;
        }
        else
        {
            slider.gameObject.SetActive(false);
            Debug.LogWarning($"VCA not found, disabling {slider.name} slider");
        }
    }

    public void SetMasterVolume(float volume)
    {
        SetVcaVolume(masterVca, volume, "MasterVolume");
    }

    public void SetSFXVolume(float volume)
    {
        SetVcaVolume(sfxVca, volume, "SFXVolume");
    }

    public void SetMusicVolume(float volume)
    {
        SetVcaVolume(musicVca, volume, "MusicVolume");
    }

    private void SetVcaVolume(FMOD.Studio.VCA vca, float volume, string prefsKey)
    {
        if (!vca.isValid()) return;

        volume = Mathf.Clamp01(volume);
        vca.setVolume(volume);
        PlayerPrefs.SetFloat(prefsKey, volume);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
    }
}