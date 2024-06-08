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
    /// <summary>
    /// ������Ϸ������
    /// </summary>
    public class KitchenGameMultiPlayer : NetworkBehaviour
    {
        /// <summary>
        /// ÿ��������������
        /// </summary>
        public const int MAX_PLAYER_AMOUNT = 4;
        private const string PLAYER_PREFS_PLAYER_NAME_MULTI_PLAYER = "PlayerName_MultiPlayer";

        public static KitchenGameMultiPlayer Instance { get; private set; }

        [SerializeField] private KitchenObjectListSO kitchenObjectListSo;
        [SerializeField] private List<Color> playerColorList;

        public event EventHandler OnTryingJoinGame;
        public event EventHandler OnFailedJoinGame;
        public event EventHandler OnPlayerDataNetworkListChanged;
        public event EventHandler OnKickPlayer;

        /// <summary>
        /// �ڷ���˴洢PlayerData��ÿһ��PlayerData��Ӧһ��Player
        /// </summary>
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

            _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTI_PLAYER, "Player_" + UnityEngine.Random.Range(100, 1000));

            DontDestroyOnLoad(this);
        }
        /// <summary>
        /// ������������ͱ仯ʱ
        /// </summary>
        /// <param name="changeEvent"></param>
        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// PlayerIndex: 0,1,2,3
        /// PlayerIndex�Ƿ��Ѿ����ӵ���Ϸ
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public bool IsPlayerIndexConnected(int playerIndex) => playerIndex < _playerDataNetworkList.Count;

        /// <summary>
        /// ��Host������Ϸ
        /// </summary>
        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        }
        /// <summary>
        /// ��Server�����ӵ���Ϸʱ
        /// </summary>
        /// <param name="clientId"></param>
        private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
        {
            // ���PlayerData��_playerDataNetworkList
            var colorId = GetFirstAvalableColorId();
            var playerData = new PlayerData() { ClientId = clientId, ColorId = colorId };
            // Add Connected Client
            _playerDataNetworkList.Add(playerData);
            SetPlayerInfoServerRpc(PlayerName, AuthenticationService.Instance.PlayerId);
        }
        /// <summary>
        /// ��Server�˶Ͽ���Ϸʱ
        /// </summary>
        /// <param name="clientId"></param>
        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
        {
            // _playerDataNetworkList���Ƴ���Ӧ��clientId��PlayerData
            // Remove Disconnected Client
            RemovePlayerData(clientId);
        }

        /// <summary>
        /// �Ƿ����� Client ��;������Ϸ
        /// ��Ҫ��NeworkManager�е�Approveѡ��ѡ��
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            //����������CharacterSelectScene(��ɫѡ�񳡾�)���ܾ�����
            if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
            {
                response.Approved = false;
                response.Reason = "Game Has Already Started!";
                return;
            }
            // ����Ϸ�Ѿ��ﵽ�������ʱ���ܾ�����
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
            {
                response.Approved = false;
                response.Reason = "Game Is Full!";
                return;
            }
            response.Approved = true;
        }
        /// <summary>
        /// �������
        /// �߳����
        /// </summary>
        /// <param name="clientId"></param>
        public void KickPlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager_Server_OnClientDisconnectCallback(clientId);
        }
        /// <summary>
        /// ��Client���ӵ���Ϸ
        /// </summary>
        public void StartClient()
        {
            OnTryingJoinGame?.Invoke(this, EventArgs.Empty);
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback; ;
            NetworkManager.Singleton.StartClient();
        }
        /// <summary>
        /// Client�˶Ͽ�����Callback
        /// </summary>
        /// <param name="clienId"></param>
        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clienId)
        {
            OnFailedJoinGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Client�����ӵ���ϷCallback
        /// </summary>
        /// <param name="clienId"></param>
        private void NetworkManager_Client_OnClientConnectedCallback(ulong clienId)
        {
            SetPlayerInfoServerRpc(PlayerName, AuthenticationService.Instance.PlayerId);
        }
        /// <summary>
        /// ���������Ϣ���洢�ڷ����
        /// </summary>
        /// <param name="playerName">�����</param>
        /// <param name="playerId">���Id</param>
        /// <param name="serverRpcParams"></param>
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

        /// <summary>
        /// ����KitchenObject
        /// </summary>
        /// <param name="kitchenObjectSo">��Ҫ���ɵ�KitchenObject����</param>
        /// <param name="kitchenObjectParent">KitchenObject�ĸ���Ʒ</param>
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
            /// ��ȡ������� NetworkObject�������������������Kitchen Object
            kitchenObjectGameObject.GetComponent<NetworkObject>().Spawn(true);
            var kitchenObject = kitchenObjectGameObject.GetComponent<KitchenObject>();

            kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            //kitchenObjectParentNetworkObject.Spawn(true);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }
        /// <summary>
        /// ����KitchenObject, ֻ�з����������������Ʒ
        /// </summary>
        /// <param name="kitchenObject"></param>
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

        /// <summary>
        /// ��ȡkitchenObjectSo��kitchenObjectListSo�е�����ֵ
        /// </summary>
        /// <param name="kitchenObjectSo"></param>
        /// <returns></returns>
        public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSo)
        {
            return kitchenObjectListSo.kitchenObjectSoList.IndexOf(kitchenObjectSo);
        }
        /// <summary>
        /// ��������ֵ��ȡKitchenObjectSO
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public KitchenObjectSO GetKitchenObjectSOByIndex(int index)
        {
            return kitchenObjectListSo.kitchenObjectSoList[index];
        }
        /// <summary>
        /// ����playerIndex��ȡPlayerData
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public PlayerData GetPlayerDataByPlayerIndex(int playerIndex)
        {
            return _playerDataNetworkList[playerIndex];
        }
        /// <summary>
        /// ����clientId��ȡPlayerData
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// PlayerIndex��Ӧ��Player�ǲ��Ƿ����
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public bool PlayerIndexIsServer(int playerIndex)
        {
            if (!IsPlayerIndexConnected(playerIndex))
            {
                return false;
            }
            var playerData = GetPlayerDataByPlayerIndex(playerIndex);
            return NetworkManager.ServerClientId == playerData.ClientId;
        }
        /// <summary>
        /// ����clientId��ȡPlayerData��_playerDataNetworkList�е�����ֵ
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
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
