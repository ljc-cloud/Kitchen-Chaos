using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace KitchenChaos.Network
{
    /// <summary>
    /// CharacterSelectScene Manage Player Ready
    /// </summary>
    public class CharacterSelectReady : NetworkBehaviour
    {
        private Dictionary<ulong, bool> _playerReadyDict;

        public static CharacterSelectReady Instance { get; private set; }

        public event EventHandler OnPlayerReadyChanged;

        private void Awake()
        {
            Instance = this;
            _playerReadyDict = new();
        }

        public void SetPlayerReady(bool ready)
        {
            SetPlayerReadyServerRpc(ready);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(bool ready, ServerRpcParams serverRpcParams = default)
        {
            _playerReadyDict[serverRpcParams.Receive.SenderClientId] = ready;
            SetPlayerReadyClientRpc(ready, serverRpcParams.Receive.SenderClientId);
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
                KitchenGameLobby.Instance.DeleteLobby();
                Loader.LoadNetwork(Loader.Scene.GameScene);
            }
        }

        [ClientRpc]
        private void SetPlayerReadyClientRpc(bool ready, ulong clientId)
        {
            _playerReadyDict[clientId] = ready;
            OnPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsPlayerReady(ulong clientId)
        {
            return _playerReadyDict.ContainsKey(clientId) && _playerReadyDict[clientId];
        }

        public bool IsLocalPlayerReady()
        {
            return _playerReadyDict.ContainsKey(NetworkManager.Singleton.LocalClientId) && _playerReadyDict[NetworkManager.Singleton.LocalClientId];
        }

    }
}
