using KitchenChaos.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        private const string READY_TEXT = "READY";
        private const string NOT_READY_TEXT = "NOT READY";

        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.LeaveLobby();
                Loader.Load(Loader.Scene.MainMenuScene);
            });
            readyButton.onClick.AddListener(() =>
            {
                if (!CharacterSelectReady.Instance.IsLocalPlayerReady())
                {
                    CharacterSelectReady.Instance.SetPlayerReady(true);
                    readyText.text = NOT_READY_TEXT;
                }
                else
                {
                    CharacterSelectReady.Instance.SetPlayerReady(false);
                    readyText.text = READY_TEXT;
                }
            });
        }

        private void Start()
        {
            var joinedlobby = KitchenGameLobby.Instance.JoinedLobby;
            lobbyNameText.text = $"Lobby Name: {joinedlobby.Name}";
            lobbyCodeText.text = $"Lobby Code: {joinedlobby.LobbyCode}";
        }
    }

}
