using KitchenChaos.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{

    public class ConnectionResponseUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            KitchenGameMultiPlayer.Instance.OnFailedJoinGame += KitchenGameMultiPlayer_OnFailedJoinGame;

            closeButton.onClick.AddListener(() =>
            {
                Hide();
            });
            Hide();
        }

        private void KitchenGameMultiPlayer_OnFailedJoinGame(object sender, System.EventArgs e)
        {
            messageText.text = NetworkManager.Singleton.DisconnectReason;
            if (string.IsNullOrEmpty(NetworkManager.Singleton.DisconnectReason))
            {
                messageText.text = "Failed To Connect!";
            }
            Show();
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);

        private void OnDestroy()
        {
            KitchenGameMultiPlayer.Instance.OnFailedJoinGame -= KitchenGameMultiPlayer_OnFailedJoinGame;
        }
    }
}