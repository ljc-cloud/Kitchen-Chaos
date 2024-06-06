using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace KitchenChaos.Network
{
    public class KitchenGameLobby : MonoBehaviour
    {
        public static KitchenGameLobby Instance { get; private set; }

        private Lobby _joinedLobby;

        public Lobby JoinedLobby => _joinedLobby;

        private float _heartBeatTImer;

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
        }

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

        private bool IsLobbyHost()
        {
            return _joinedLobby != null && AuthenticationService.Instance.PlayerId == _joinedLobby.HostId;
        }

        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                return;
            }
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 100).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }

        public async Task CreateLobby(string lobbyName, bool isPrivate)
        {
            try
            {
                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });

                KitchenGameMultiPlayer.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async Task QuickJoinLobby()
        {
            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                KitchenGameMultiPlayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async Task JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

                KitchenGameMultiPlayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

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

