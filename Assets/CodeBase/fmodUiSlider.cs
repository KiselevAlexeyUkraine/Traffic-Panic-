using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class FMODVolumeController : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        // Подписываемся на изменения слайдеров
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);

        // Инициализация начальных значений (если нужно)
        SetMasterVolume(masterSlider.value);
        SetSFXVolume(sfxSlider.value);
        SetMusicVolume(musicSlider.value);
    }

    private void SetMasterVolume(float volume)
    {
        RuntimeManager.StudioSystem.setParameterByName("master_volume", volume);
    }

    private void SetSFXVolume(float volume)
    {
        RuntimeManager.StudioSystem.setParameterByName("sfx_volume", volume);
    }

    private void SetMusicVolume(float volume)
    {
        RuntimeManager.StudioSystem.setParameterByName("music_volume", volume);
    }

    // Сохранение и загрузка (опционально)
    public void SaveVolumes()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void LoadVolumes()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }
}