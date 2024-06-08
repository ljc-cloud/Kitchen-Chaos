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
    /// 多人游戏管理类
    /// </summary>
    public class KitchenGameMultiPlayer : NetworkBehaviour
    {
        /// <summary>
        /// 每个房间最多玩家数
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
        /// 在服务端存储PlayerData，每一个PlayerData对应一个Player
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
        /// 当玩家数量发送变化时
        /// </summary>
        /// <param name="changeEvent"></param>
        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// PlayerIndex: 0,1,2,3
        /// PlayerIndex是否已经连接到游戏
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public bool IsPlayerIndexConnected(int playerIndex) => playerIndex < _playerDataNetworkList.Count;

        /// <summary>
        /// 以Host链接游戏
        /// </summary>
        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        }
        /// <summary>
        /// 当Server端连接到游戏时
        /// </summary>
        /// <param name="clientId"></param>
        private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
        {
            // 添加PlayerData到_playerDataNetworkList
            var colorId = GetFirstAvalableColorId();
            var playerData = new PlayerData() { ClientId = clientId, ColorId = colorId };
            // Add Connected Client
            _playerDataNetworkList.Add(playerData);
            SetPlayerInfoServerRpc(PlayerName, AuthenticationService.Instance.PlayerId);
        }
        /// <summary>
        /// 当Server端断开游戏时
        /// </summary>
        /// <param name="clientId"></param>
        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
        {
            // _playerDataNetworkList中移除对应的clientId的PlayerData
            // Remove Disconnected Client
            RemovePlayerData(clientId);
        }

        /// <summary>
        /// 是否允许 Client 中途加入游戏
        /// 需要将NeworkManager中的Approve选项选中
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            //当场景不是CharacterSelectScene(角色选择场景)，拒绝加入
            if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
            {
                response.Approved = false;
                response.Reason = "Game Has Already Started!";
                return;
            }
            // 当游戏已经达到最大人数时，拒绝加入
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
            {
                response.Approved = false;
                response.Reason = "Game Is Full!";
                return;
            }
            response.Approved = true;
        }
        /// <summary>
        /// 网络层面
        /// 踢出玩家
        /// </summary>
        /// <param name="clientId"></param>
        public void KickPlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager_Server_OnClientDisconnectCallback(clientId);
        }
        /// <summary>
        /// 以Client连接到游戏
        /// </summary>
        public void StartClient()
        {
            OnTryingJoinGame?.Invoke(this, EventArgs.Empty);
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback; ;
            NetworkManager.Singleton.StartClient();
        }
        /// <summary>
        /// Client端断开连接Callback
        /// </summary>
        /// <param name="clienId"></param>
        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clienId)
        {
            OnFailedJoinGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Client端连接到游戏Callback
        /// </summary>
        /// <param name="clienId"></param>
        private void NetworkManager_Client_OnClientConnectedCallback(ulong clienId)
        {
            SetPlayerInfoServerRpc(PlayerName, AuthenticationService.Instance.PlayerId);
        }
        /// <summary>
        /// 设置玩家信息，存储在服务端
        /// </summary>
        /// <param name="playerName">玩家名</param>
        /// <param name="playerId">玩家Id</param>
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
        /// 生成KitchenObject
        /// </summary>
        /// <param name="kitchenObjectSo">需要生成的KitchenObject菜谱</param>
        /// <param name="kitchenObjectParent">KitchenObject的父物品</param>
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
            /// 获取网络组件 NetworkObject，并在网络上生成这个Kitchen Object
            kitchenObjectGameObject.GetComponent<NetworkObject>().Spawn(true);
            var kitchenObject = kitchenObjectGameObject.GetComponent<KitchenObject>();

            kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            //kitchenObjectParentNetworkObject.Spawn(true);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }
        /// <summary>
        /// 销毁KitchenObject, 只有服务端能销毁网络物品
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
        /// 获取kitchenObjectSo在kitchenObjectListSo中的索引值
        /// </summary>
        /// <param name="kitchenObjectSo"></param>
        /// <returns></returns>
        public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSo)
        {
            return kitchenObjectListSo.kitchenObjectSoList.IndexOf(kitchenObjectSo);
        }
        /// <summary>
        /// 根据索引值获取KitchenObjectSO
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public KitchenObjectSO GetKitchenObjectSOByIndex(int index)
        {
            return kitchenObjectListSo.kitchenObjectSoList[index];
        }
        /// <summary>
        /// 根据playerIndex获取PlayerData
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public PlayerData GetPlayerDataByPlayerIndex(int playerIndex)
        {
            return _playerDataNetworkList[playerIndex];
        }
        /// <summary>
        /// 根据clientId获取PlayerData
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
        /// PlayerIndex对应的Player是不是服务端
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
        /// 根据clientId获取PlayerData在_playerDataNetworkList中的索引值
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
