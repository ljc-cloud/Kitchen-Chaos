using KitchenChaos.Player;
using System;
using UnityEngine;

namespace KitchenChaos.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private float gamePlayingTimerMax;

        private enum State
        {
            WaitingToStart,
            CountdownToStart,
            GamePlaying,
            Gameover
        }
        private State _state;
        public event EventHandler OnStateChanged;

        private float _countdownToStartTimer = 3f;
        private float _gamePlayingTimer = 120f;


        public event EventHandler OnGamePause;
        public event EventHandler OnGameUnPause;
        public event EventHandler OnTutorialEnd;

        public bool IsGamePlaying => _state == State.GamePlaying;
        public bool IsCountdownToStart => _state == State.CountdownToStart;

        public bool IsGameover => _state == State.Gameover;

        public bool IsGamePause { get; private set; }

        public float CountdownToStartTimer => _countdownToStartTimer;

        private bool _isReadingTutorial;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _state = State.WaitingToStart;
            _gamePlayingTimer = gamePlayingTimerMax;
            IsGamePause = false;
            _isReadingTutorial = true;
            GameInput.Instance.OnGamePauseAction += GameManagerOnGamePauseAction;
            GameInput.Instance.OnInteractAction += GameManagerOnInteractAction;
        }

        private void GameManagerOnInteractAction(object sender, EventArgs e)
        {
            _isReadingTutorial = false;
        }

        private void GameManagerOnGamePauseAction(object sender, EventArgs e)
        {
            ToogleGamePause();
        }

        private void Update()
        {
            switch (_state)
            {
                case State.WaitingToStart:
                    if (!_isReadingTutorial)
                    {
                        _state = State.CountdownToStart;
                        OnStateChanged?.Invoke(this, EventArgs.Empty);
                        OnTutorialEnd?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case State.CountdownToStart:
                    _countdownToStartTimer -= Time.deltaTime;
                    if (_countdownToStartTimer < 0)
                    {
                        _state = State.GamePlaying;
                        OnStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case State.GamePlaying:
                    _gamePlayingTimer -= Time.deltaTime;
                    if (_gamePlayingTimer < 0)
                    {
                        _state = State.Gameover;
                        OnStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case State.Gameover: break;
            }
        }

        public void ToogleGamePause()
        {
            if (IsGamePause)
            {
                Time.timeScale = 0;
                OnGamePause?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1;
                OnGameUnPause?.Invoke(this, EventArgs.Empty);
            }
            IsGamePause = !IsGamePause;
        }

        public float GetGamePlayingTimerNormalized() => 1 - (_gamePlayingTimer / gamePlayingTimerMax);

    }
}
