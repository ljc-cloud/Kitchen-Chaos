using KitchenChaos.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button mainmenuButton;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button quickJoinButton;
        [SerializeField] private Button joinCodeButton;
        [SerializeField] private TMP_InputField joinCodeInputField;
        [SerializeField] private TMP_InputField playNameInputField;
        [SerializeField] private LobbyCreateUI lobbyCreateUI;

        private void Awake()
        {
            mainmenuButton.onClick.AddListener(() =>
            {
                Loader.Load(Loader.Scene.MainMenuScene);
            });
            createLobbyButton.onClick.AddListener(() =>
            {
                lobbyCreateUI.Show();
                KitchenGameMultiPlayer.Instance.PlayerName = playNameInputField.text;
            });
            quickJoinButton.onClick.AddListener(async () =>
            {
                await KitchenGameLobby.Instance.QuickJoinLobby();
                KitchenGameMultiPlayer.Instance.PlayerName = playNameInputField.text;
            });
            joinCodeButton.onClick.AddListener(async () =>
            {
                await KitchenGameLobby.Instance.JoinLobbyByCode(joinCodeInputField.text);
                KitchenGameMultiPlayer.Instance.PlayerName = playNameInputField.text;
            });
        }
    }
}