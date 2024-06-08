using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KitchenChaos.Network
{
    /// <summary>
    /// Lobby ���������
    /// </summary>
    public class KitchenGameLobby : MonoBehaviour
    {
        private const string RELAY_JOIN_CODE_KEY = "relay";
        public static KitchenGameLobby Instance { get; private set; }

        private Lobby _joinedLobby;

        /// <summary>
        /// �Ѿ�����ķ���
        /// </summary>
        public Lobby JoinedLobby => _joinedLobby;

        private float _heartBeatTImer;
        private float _listLobbyTimer;

        public event EventHandler OnJoinLobyStarted;
        public event EventHandler OnJoinLobbyFailed;
        public event EventHandler OnQuickJoinFailed;
        public event EventHandler OnCreateLobbyStarted;
        public event EventHandler OnCreateLobbyFailed;
        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : EventArgs
        {
            public List<Lobby> LobbyList;
        }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUnityAuthentication();
        }


        private void Update()
        {
            // Only Host Need Send HeartBeat To Keep Lobby Exist
            HandleHeartBeat();
            HandlePeriodicListLobby();
        }
        /// <summary>
        /// ÿ��һ��ʱ���ѯLobbyList
        /// </summary>
        private void HandlePeriodicListLobby()
        {
            if (_joinedLobby != null ||
                !AuthenticationService.Instance.IsSignedIn ||
                SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString())
            {
                return;
            }
            _listLobbyTimer -= Time.deltaTime;
            if (_listLobbyTimer <= 0f)
            {
                float listLobbyTimerMax = 3f;
                _listLobbyTimer = listLobbyTimerMax;
                ListLobbies();
            }
        }

        /// <summary>
        /// ÿ��һ��ʱ�䷢��������ʹ�������Active״̬
        /// </summary>
        private void HandleHeartBeat()
        {
            if (IsLobbyHost())
            {
                _heartBeatTImer -= Time.deltaTime;
                if (_heartBeatTImer <= 0f)
                {
                    float heartBeatTiemrMax = 10f;
                    _heartBeatTImer = heartBeatTiemrMax;

                    // Send Hear Beat To Keep Lobby Exist
                    LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
                }
            }

        }

        /// <summary>
        /// �ж�����Ƿ�����������Host
        /// </summary>
        /// <returns></returns>
        private bool IsLobbyHost()
        {
            return _joinedLobby != null && AuthenticationService.Instance.PlayerId == _joinedLobby.HostId;
        }

        /// <summary>
        /// ��ʼ��Unity��֤���񣬵�¼��UnityService��ֻ��ִ��һ�Σ�
        /// </summary>
        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                return;
            }
            InitializationOptions initializationOptions = new InitializationOptions();
            // ���ò�ͬ��ң������ã�Ĭ�ϸ����豸�б�ͬ���
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 100).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }

        /// <summary>
        /// ��ѯ���Խ���ķ���
        /// </summary>
        public async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
                queryLobbiesOptions.Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                };
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                print($"��ѯ���ɽ��뷿�䣺{queryResponse.Results.Count}");
                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
                {
                    LobbyList = queryResponse.Results
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// �����м���
        /// </summary>
        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                // ָ���������������Host�⣩
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT - 1);
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        private async Task<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        private async Task<JoinAllocation> JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                return joinAllocation;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="lobbyName">������</param>
        /// <param name="isPrivate">������</param>
        /// <returns></returns>
        public async Task CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
            if (string.IsNullOrEmpty(lobbyName))
            {
                OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
                return;
            }
            try
            {
                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });

                Allocation allocation = await AllocateRelay();

                string relayJoinCode = await GetRelayJoinCode(allocation);

                await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                    }
                });

                NetworkManager.Singleton.GetComponent<UnityTransport>().
                    SetRelayServerData(new RelayServerData(allocation, "dtls"));

                KitchenGameMultiPlayer.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// ���ټ��뷿��
        /// </summary>
        /// <returns></returns>
        public async Task QuickJoinLobby()
        {
            OnJoinLobyStarted?.Invoke(this, EventArgs.Empty);
            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                string relayJoinCode = _joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;

                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                NetworkManager.Singleton.GetComponent<UnityTransport>().
                    SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                KitchenGameMultiPlayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// ���ݷ���Id����
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        public async Task JoinLobbyById(string lobbyId)
        {
            OnJoinLobyStarted?.Invoke(this, EventArgs.Empty);
            if (string.IsNullOrEmpty(lobbyId))
            {
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
                return;
            }
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

                string relayJoinCode = _joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;

                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                NetworkManager.Singleton.GetComponent<UnityTransport>().
                    SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                KitchenGameMultiPlayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// ���ݷ���Code����
        /// </summary>
        /// <param name="lobbyCode"></param>
        /// <returns></returns>
        public async Task JoinLobbyByCode(string lobbyCode)
        {
            OnJoinLobyStarted?.Invoke(this, EventArgs.Empty);
            if (string.IsNullOrEmpty(lobbyCode))
            {
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
                return;
            }
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

                string relayJoinCode = _joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;

                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                NetworkManager.Singleton.GetComponent<UnityTransport>().
                    SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                KitchenGameMultiPlayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// ɾ������
        /// </summary>
        public async void DeleteLobby()
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                    _joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }
        /// <summary>
        /// �뿪����
        /// </summary>
        public async void LeaveLobby()
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    _joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }
        /// <summary>
        /// �߳�����
        /// </summary>
        /// <param name="playerId"></param>
        public async void KickPlayer(string playerId)
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);

                    _joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

    }
}

