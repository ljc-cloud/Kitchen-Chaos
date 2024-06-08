using KitchenChaos.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{

    public class LobbyMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            KitchenGameMultiPlayer.Instance.OnFailedJoinGame += KitchenGameMultiPlayer_OnFailedJoinGame;

            KitchenGameLobby.Instance.OnJoinLobyStarted += KitchenGameLobby_OnJoinLobyStarted;
            KitchenGameLobby.Instance.OnJoinLobbyFailed += KitchenGameLobby_OnJoinLobbyFailed;
            KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
            KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
            KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;

            closeButton.onClick.AddListener(() =>
            {
                Hide();
            });
            Hide();
        }

        private void OnDestroy()
        {
            KitchenGameMultiPlayer.Instance.OnFailedJoinGame -= KitchenGameMultiPlayer_OnFailedJoinGame;
            KitchenGameLobby.Instance.OnJoinLobyStarted -= KitchenGameLobby_OnJoinLobyStarted;
            KitchenGameLobby.Instance.OnJoinLobbyFailed -= KitchenGameLobby_OnJoinLobbyFailed;
            KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
            KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
            KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        }

        private void KitchenGameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e)
        {
            ShowMessahe("Fail To Create Lobby!");
        }

        private void KitchenGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e)
        {
            ShowMessahe("Creating Lobby...");
        }

        private void KitchenGameLobby_OnQuickJoinFailed(object sender, System.EventArgs e)
        {
            ShowMessahe("Could Not Find A Lobby To Quick Join!");
        }

        private void KitchenGameLobby_OnJoinLobbyFailed(object sender, System.EventArgs e)
        {
            ShowMessahe("Fail To Join Lobby!");
        }

        private void KitchenGameLobby_OnJoinLobyStarted(object sender, System.EventArgs e)
        {
            ShowMessahe("Joining Lobby...");
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

        private void ShowMessahe(string message)
        {
            messageText.text = message;
            Show();
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);

    }
}