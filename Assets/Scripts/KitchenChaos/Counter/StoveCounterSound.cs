using KitchenChaos.Manager;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class StoveCounterSound : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;

        private AudioSource _audioSource;

        private float _warningSoundTimer;
        private float _warningSoundTimerMax = .5f;
        private bool _playWarningSound;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }


        private void Start()
        {
            stoveCounter.OnStateChanged += StoveCounterOnStateChanged;
            stoveCounter.OnProgressbarChanged += StoveCounter_OnProgressbarChanged;
        }

        private void Update()
        {
            if (_playWarningSound)
            {
                _warningSoundTimer -= Time.deltaTime;
                if (_warningSoundTimer < 0)
                {
                    _warningSoundTimer = _warningSoundTimerMax;
                    SoundManager.Instance.PlayStoveCounterWarningSound(transform.position);
                }
            }
        }

        private void StoveCounter_OnProgressbarChanged(object sender, Interface.IHasProgress.OnProgressbarChangedEventArgs e)
        {
            _playWarningSound = stoveCounter.IsFried && e.progressNormalized > .5f;
        }

        private void StoveCounterOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            bool canPlay = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
            if (canPlay)
            {
                _audioSource.Play();
            }
            else
            {
                _audioSource.Pause();
            }
        }
    }
}
