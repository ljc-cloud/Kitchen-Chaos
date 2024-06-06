using KitchenChaos.Interface;
using KitchenChaos.Manager;
using KitchenChaos.Player;
using KitchenChaos.SO;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

namespace KitchenChaos.Network
{
    public class KitchenGameMultiPlayer : NetworkBehaviour
    {
        public const int MAX_PLAYER_AMOUNT = 4;
        private const string PLAYER_PREFS_PLAYER_NAME_MULTI_PLAYER = "PlayerName_MultiPlayer";

        public static KitchenGameMultiPlayer Instance { get; private set; }

        [SerializeField] private KitchenObjectListSO kitchenObjectListSo;
        [SerializeField] private List<Color> playerColorList;

        public event EventHandler OnTryingJoinGame;
        public event EventHandler OnFailedJoinGame;
        public event EventHandler OnPlayerDataNetworkListChanged;

        private NetworkList<PlayerData> _playerDataNetworkList;

        private string _playerName;

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTI_PLAYER, value);
            }
        }

        private void Awake()
        {
            Instance = this;
            _playerDataNetworkList = new NetworkList<PlayerData>();
            _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

            _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTI_PLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));

            DontDestroyOnLoad(this);
        }

        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }
        public bool IsPlayerIndexConnected(int playerIndex) => playerIndex < _playerDataNetworkList.Count;

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        }
        private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
        {
            var colorId = GetFirstAvalableColorId();
            var playerData = new PlayerData() { ClientId = clientId, ColorId = colorId };
            // Add Connected Client
            _playerDataNetworkList.Add(playerData);
            SetPlayerInfoServerRpc(PlayerName, AuthenticationService.Instance.PlayerId);
        }
        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
        {
            // Remove Disconnected Client
            RemovePlayerData(clientId);
        }

        /// <summary>
        /// 是否允许 Client 中途加入游戏
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
            {
                response.Approved = false;
                response.Reason = "Game Has Already Started!";
                return;
            }
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
            {
                response.Approved = false;
                response.Reason = "Game Is Full!";
                return;
            }
            response.Approved = true;
        }

        public void KickPlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            RemovePlayerData(clientId);
        }

        public void StartClient()
        {
            OnTryingJoinGame?.Invoke(this, EventArgs.Empty);
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback; ;
            NetworkManager.Singleton.StartClient();
        }

        private void NetworkManager_Client_OnClientConnectedCallback(ulong clienId)
        {
            SetPlayerInfoServerRpc(PlayerName, AuthenticationService.Instance.PlayerId);
        }
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerInfoServerRpc(string playerName, string playerId, ServerRpcParams serverRpcParams = default) 
        {
            var playerDataIndex = GetPlayerDataIndexByClientId(serverRpcParams.Receive.SenderClientId);
            var playerData = _playerDataNetworkList[playerDataIndex];
            playerData.PlayerName = playerName;
            playerData.PlayerId = playerId;

            _playerDataNetworkList[playerDataIndex] = playerData;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
        {
            var playerDataIndex = GetPlayerDataIndexByClientId(serverRpcParams.Receive.SenderClientId);
            var playerData = _playerDataNetworkList[playerDataIndex];
            playerData.PlayerName = playerName;

            _playerDataNetworkList[playerDataIndex] = playerData;
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clienId)
        {
            OnFailedJoinGame?.Invoke(this, EventArgs.Empty);
        }

        public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
        {
            if (GameManager.Instance.IsGamePause) return;
            var index = GetKitchenObjectIndex(kitchenObjectSo);
            SpawnKitchenObjectServerRpc(index, kitchenObjectParent.GetNetworkObject());
        }
        [ServerRpc(RequireOwnership = false)]
        private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex, NetworkObjectReference kitchenObjectParentNetworkReference)
        {
            var kitchenObjectSo = GetKitchenObjectSOByIndex(kitchenObjectSoIndex);
            var kitchenObjectGameObject = Instantiate(kitchenObjectSo.prefab);
            kitchenObjectGameObject.GetComponent<NetworkObject>().Spawn(true);
            var kitchenObject = kitchenObjectGameObject.GetComponent<KitchenObject>();

            kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            //kitchenObjectParentNetworkObject.Spawn(true);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }

        public void DestroyKitchenObject(KitchenObject kitchenObject)
        {
            DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
        }
        [ServerRpc(RequireOwnership = false)]
        private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkReference)
        {
            kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
            var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
            ClearKitchenObjectParentClientRpc(kitchenObjectNetworkReference);
            kitchenObject.DestroySelf();
        }
        [ClientRpc]
        private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkReference)
        {
            kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
            var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
            kitchenObject.ClearKitchenObjectOnParent();
        }

        public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSo)
        {
            return kitchenObjectListSo.kitchenObjectSoList.IndexOf(kitchenObjectSo);
        }

        public KitchenObjectSO GetKitchenObjectSOByIndex(int index)
        {
            return kitchenObjectListSo.kitchenObjectSoList[index];
        }

        public PlayerData GetPlayerDataByPlayerIndex(int playerIndex)
        {
            return _playerDataNetworkList[playerIndex];
        }

        public PlayerData GetPlayerDataByClientId(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++)
            {
                if (_playerDataNetworkList[i].ClientId == clientId)
                {
                    return _playerDataNetworkList[i];
                }
            }
            return default;
        }

        public int GetPlayerDataIndexByClientId(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++)
            {
                if (_playerDataNetworkList[i].ClientId == clientId)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsColorAvalable(int colorId)
        {
            foreach (var playerData in _playerDataNetworkList)
            {
                if (playerData.ColorId == colorId)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetFirstAvalableColorId()
        {
            for (int i = 0; i < playerColorList.Count; i++)
            {
                if (IsColorAvalable(i))
                {
                    return i;
                }
            }
            return -1;
        }

        public Color GetPlayerColorByIndex(int playerIndex)
        {
            return playerColorList[playerIndex];
        }

        public void ChangePlayerColor(int colorId)
        {
            ChangePlayerColorServerRpc(colorId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
        {
            var playerDataIndex = GetPlayerDataIndexByClientId(serverRpcParams.Receive.SenderClientId);
            if (playerDataIndex == -1)
            {
                return;
            }
            var playerData = _playerDataNetworkList[playerDataIndex];
            playerData.ColorId = colorId;
            _playerDataNetworkList[playerDataIndex] = playerData;
        }

        public void RemovePlayerData(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++)
            {
                if (_playerDataNetworkList[i].ClientId == clientId)
                {
                    _playerDataNetworkList.RemoveAt(i);
                }
            }
        }

    }

}
