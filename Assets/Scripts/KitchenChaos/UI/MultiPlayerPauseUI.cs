using KitchenChaos.Manager;
using System;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class MultiPlayerPauseUI : MonoBehaviour
    {

        private void Start()
        {
            GameManager.Instance.OnMultiPlayerGamePause += GameManager_OnMultiPlayerGamePause;
            GameManager.Instance.OnMultiPlayerGameUnpause += GameManager_OnMultiPlayerUnpause;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            Hide();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnMultiPlayerGamePause += GameManager_OnMultiPlayerGamePause;
            GameManager.Instance.OnMultiPlayerGameUnpause += GameManager_OnMultiPlayerUnpause;
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                Hide();
            }
        }

        private void GameManager_OnMultiPlayerUnpause(object sender, EventArgs e)
        {
            Hide();
        }

        private void GameManager_OnMultiPlayerGamePause(object sender, System.EventArgs e)
        {
            Show();
        }


        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}

