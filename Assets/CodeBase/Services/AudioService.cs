using UnityEngine;
using UnityEngine.Audio;

namespace Codebase.Services
{
    public class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _pauseClick;

        private const string MasterVolumeKey = "MasterVolume";
        private const string SoundsVolumeKey = "SoundsVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const float DefaultVolume = 0.5f;

        public float SavedMasterVolume { get; private set; }
        public float SavedSoundsVolume { get; private set; }
        public float SavedMusicVolume { get; private set; }

        private void Awake()
        {
            LoadAudioSettings();
        }

        private void Start()
        {
            ApplyVolume();
        }

        public void PlayHoverSound()
        {
            if (_audioSource != null && _buttonClick != null)
            {
                _audioSource.PlayOneShot(_buttonClick, SavedSoundsVolume);
            }
        }

        public void PlayClickSound()
        {
            if (_audioSource != null && _pauseClick != null)
            {
                _audioSource.PlayOneShot(_pauseClick, SavedSoundsVolume);
            }
        }

        public void SetMasterVolume(float value)
        {
            SavedMasterVolume = Mathf.Clamp(value, 0.0001f, 1f);
            SetVolumeToMixer("MasterVolume", SavedMasterVolume);
            PlayerPrefs.SetFloat(MasterVolumeKey, SavedMasterVolume);
        }

        public void SetSoundsVolume(float value)
        {
            SavedSoundsVolume = Mathf.Clamp(value, 0.0001f, 1f);
            SetVolumeToMixer("SoundsVolume", SavedSoundsVolume);
            PlayerPrefs.SetFloat(SoundsVolumeKey, SavedSoundsVolume);
        }

        public void SetMusicVolume(float value)
        {
            SavedMusicVolume = Mathf.Clamp(value, 0.0001f, 1f);
            SetVolumeToMixer("MusicVolume", SavedMusicVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, SavedMusicVolume);
        }

        public void SaveVolumes()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, SavedMasterVolume);
            PlayerPrefs.SetFloat(SoundsVolumeKey, SavedSoundsVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, SavedMusicVolume);
            PlayerPrefs.Save();
        }

        private void LoadAudioSettings()
        {
            SavedMasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, DefaultVolume);
            SavedSoundsVolume = PlayerPrefs.GetFloat(SoundsVolumeKey, DefaultVolume);
            SavedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultVolume);
        }

        private void ApplyVolume()
        {
            SetVolumeToMixer("MasterVolume", SavedMasterVolume);
            SetVolumeToMixer("SoundsVolume", SavedSoundsVolume);
            SetVolumeToMixer("MusicVolume", SavedMusicVolume);
        }

        private void SetVolumeToMixer(string parameter, float value)
        {
            float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            _mixer.SetFloat(parameter, volume);
        }
    }
}
