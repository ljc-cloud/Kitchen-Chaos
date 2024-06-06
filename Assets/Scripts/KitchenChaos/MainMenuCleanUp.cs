using KitchenChaos.Network;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos
{
    public class MainMenuCleanUp : MonoBehaviour
    {
        private void Awake()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            if (KitchenGameMultiPlayer.Instance != null)
            {
                Destroy(KitchenGameMultiPlayer.Instance.gameObject);
            }
            if (KitchenGameLobby.Instance != null)
            {
                Destroy(KitchenGameLobby.Instance.gameObject);
            }
        }
    }
}

