using KitchenChaos.Network;
using KitchenChaos.Player;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Manager
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameObject playerPrefab;

        private enum State
        {
            WaitingToStart,
            CountdownToStart,
            GamePlaying,
            Gameover
        }
        private NetworkVariable<State> _state = new NetworkVariable<State>(State.WaitingToStart);
        public event EventHandler OnStateChanged;

        private const float _gamePlayingTimerMax = 300f;

        private NetworkVariable<float> _countdownToStartTimer = new NetworkVariable<float>(3f);
        private NetworkVariable<float> _gamePlayingTimer = new NetworkVariable<float>(_gamePlayingTimerMax);


        public event EventHandler OnLocalGamePause;
        public event EventHandler OnLocalGameUnpause;
        public event EventHandler OnMultiPlayerGamePause;
        public event EventHandler OnMultiPlayerGameUnpause;
        public event EventHandler OnLocalPlayerReady;

        public bool IsWaitingToStart => _state.Value == State.WaitingToStart;
        public bool IsGamePlaying => _state.Value == State.GamePlaying;
        public bool IsCountdownToStart => _state.Value == State.CountdownToStart;

        public bool IsGameover => _state.Value == State.Gameover;

        public bool IsLocalGamePause { get; private set; }
        private NetworkVariable<bool> _isGamePause = new NetworkVariable<bool>(false);
        public bool IsGamePause => _isGamePause.Value;

        public float CountdownToStartTimer => _countdownToStartTimer.Value;

        public bool IsLocalPlayerReady { get; private set; }

        private Dictionary<ulong, bool> _playerReadyDict;
        private Dictionary<ulong, bool> _playerPauseDict;

        private bool _autoGamePause;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            IsLocalGamePause = false;
            _playerReadyDict = new();
            _playerPauseDict = new();

            GameInput.Instance.OnGamePauseAction += GameInput_OnGamePauseAction;
            GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        }

        public override void OnNetworkSpawn()
        {
            _state.OnValueChanged += State_OnValueChanged;
            _isGamePause.OnValueChanged += IsGamePause_OnValueChanged;
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
                // 当所有client场景完成加载
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameInput.Instance.OnGamePauseAction -= GameInput_OnGamePauseAction;
            GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= NetworkManager_OnLoadEventCompleted;
            }
        }

        private void NetworkManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerGameObject = Instantiate(playerPrefab);
                playerGameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong obj)
        {
            _autoGamePause = true;
        }

        /// <summary>
        /// MultiPlayerGamePause
        /// </summary>
        /// <param name="previousValue"></param>
        /// <param name="newValue"></param>
        private void IsGamePause_OnValueChanged(bool previousValue, bool newValue)
        {
            if (_isGamePause.Value)
            {
                Time.timeScale = 0f;
                OnMultiPlayerGamePause?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1f;
                OnMultiPlayerGameUnpause?.Invoke(this, EventArgs.Empty);
            }
        }

        private void State_OnValueChanged(State previousValue, State newValue)
        {
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
        private void GameInput_OnInteractAction(object sender, EventArgs e)
        {
            if (_state.Value == State.WaitingToStart)
            {
                IsLocalPlayerReady = true;
                OnLocalPlayerReady?.Invoke(this, EventArgs.Empty);
                SetPlayerReadyServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

            bool allPlayerReady = true;
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerReadyDict.ContainsKey(clientId) || !_playerReadyDict[clientId])
                {
                    allPlayerReady = false;
                    break;
                }
            }
            print($"All Player Ready: {allPlayerReady}");
            if (allPlayerReady)
            {
                _state.Value = State.CountdownToStart;
            }
        }

        private void GameInput_OnGamePauseAction(object sender, EventArgs e)
        {
            ToogleGamePause();
        }

        private void Update()
        {
            if (!IsServer) return;
            switch (_state.Value)
            {
                case State.WaitingToStart:
                    break;
                case State.CountdownToStart:
                    _countdownToStartTimer.Value -= Time.deltaTime;
                    if (_countdownToStartTimer.Value < 0)
                    {
                        _state.Value = State.GamePlaying;

                    }
                    break;
                case State.GamePlaying:
                    _gamePlayingTimer.Value -= Time.deltaTime;
                    if (_gamePlayingTimer.Value < 0)
                    {
                        _state.Value = State.Gameover;
                    }
                    break;
                case State.Gameover: break;
            }
        }
        private void LateUpdate()
        {
            if (_autoGamePause)
            {
                _autoGamePause = false;
                TestGamePause();
            }
        }

        public void ToogleGamePause()
        {
            if (IsLocalGamePause)
            {
                PauseGameServerRpc();
                OnLocalGamePause?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UnpauseGameServerRpc();
                OnLocalGameUnpause?.Invoke(this, EventArgs.Empty);
            }
            IsLocalGamePause = !IsLocalGamePause;
        }

        [ServerRpc(RequireOwnership = false)]
        private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerPauseDict[serverRpcParams.Receive.SenderClientId] = true;
            TestGamePause();
        }
        [ServerRpc(RequireOwnership = false)]
        private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerPauseDict[serverRpcParams.Receive.SenderClientId] = false;
            TestGamePause();
        }

        private void TestGamePause()
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (_playerPauseDict.ContainsKey(clientId) && _playerPauseDict[clientId])
                {
                    _isGamePause.Value = true;
                    return;
                }
            }
            _isGamePause.Value = false;
        }

        public float GetGamePlayingTimerNormalized() => 1 - (_gamePlayingTimer.Value / _gamePlayingTimerMax);

    }
}
