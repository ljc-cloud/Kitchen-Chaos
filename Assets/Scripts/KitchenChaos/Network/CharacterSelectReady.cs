using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Network
{
    public class CharacterSelectReady : NetworkBehaviour
    {
        private Dictionary<ulong, bool> _playerReadyDict;

        public static CharacterSelectReady Instance {  get; private set; }

        private void Awake()
        {
            Instance = this;
            _playerReadyDict = new();
        }

        public void SetPlayerReady()
        {
            SetPlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

            bool allReady = true;
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerReadyDict.ContainsKey(clientId) || !_playerReadyDict[clientId])
                {
                    allReady = false;
                    break;
                }
            }
            if (allReady)
            {
                Loader.LoadNetwork(Loader.Scene.GameScene);
            }
        }
    }
}
