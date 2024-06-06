using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class HostDisconnectedUI : MonoBehaviour
    {
        [SerializeField] private Button playAgainButton;
        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            playAgainButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.MainMenuScene); });
            Hide();
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                Show();
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
            }
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}