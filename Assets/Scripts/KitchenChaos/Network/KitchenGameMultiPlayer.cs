using KitchenChaos.Interface;
using KitchenChaos.Manager;
using KitchenChaos.SO;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace KitchenChaos.Network
{
    public class KitchenGameMultiPlayer : NetworkBehaviour
    {
        private const int MAX_PLAYER_AMOUNT = 4;

        public static KitchenGameMultiPlayer Instance { get; private set; }

        [SerializeField] private KitchenObjectListSO kitchenObjectListSo;

        public event EventHandler OnTryingJoinGame;
        public event EventHandler OnFailedJoinGame;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.StartHost();
        }

        /// <summary>
        /// 是否允许 client 中途加入游戏
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
            //if (GameManager.Instance.IsWaitingToStart)
            //{
            //    response.Approved = true;
            //    response.CreatePlayerObject = true;
            //}
            //else
            //{
            //    response.Approved = false;
            //}
            response.Approved = true;
        }

        public void StartClient()
        {
            OnTryingJoinGame?.Invoke(this, EventArgs.Empty);
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartClient();
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            OnFailedJoinGame?.Invoke(this, EventArgs.Empty);
        }

        public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
        {
            if (GameManager.Instance.IsGamePause) return;
            var index = GetKitchenObjectIndex(kitchenObjectSo);
            SpawnKitchenObjectServerRpc(index, kitchenObjectParent.GetNetworkObject());
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

        public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSo)
        {
            return kitchenObjectListSo.kitchenObjectSoList.IndexOf(kitchenObjectSo);
        }

        public KitchenObjectSO GetKitchenObjectSOByIndex(int index)
        {
            return kitchenObjectListSo.kitchenObjectSoList[index];
        }
    }

}
