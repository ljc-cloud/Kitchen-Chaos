using UnityEngine;

namespace KitchenChaos.Manager
{
    /// <summary>
    /// MusicManager
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        private const string PlayerPrefsMusicVolume = "Music:";
        public static MusicManager Instance { get; private set; }

        private AudioSource _audioSource;

        private float _volume = .9f;


        private void Awake()
        {
            Instance = this;
            _audioSource = GetComponent<AudioSource>();

            var volume = PlayerPrefs.GetFloat(PlayerPrefsMusicVolume, .9f);
            _volume = volume;
            _audioSource.volume = volume;
        }

        private void Start()
        {
            _audioSource.volume = _volume;
        }

        public void ChangeVolume()
        {
            _volume += 0.1f;
            if (_volume > 1f) _volume = 0;
            _audioSource.volume = _volume;
            PlayerPrefs.SetFloat(PlayerPrefsMusicVolume, _volume);
            PlayerPrefs.Save();
        }

        public float GetVolume() => _volume;
    }

}
