using KitchenChaos.Network;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
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
        [SerializeField] private GameObject lobbyListContainer;
        [SerializeField] private GameObject lobbyTemplate;

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

        private void Start()
        {
            KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;
            lobbyTemplate.SetActive(false);
        }
        private void OnDestroy()
        {
            KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
        }

        private void UpdateLobbyList(List<Lobby> lobbyList)
        {
            foreach (Transform child in lobbyListContainer.transform)
            {
                if (child == lobbyTemplate.transform) continue;
                Destroy(child.gameObject);
            }

            foreach (var lobby in lobbyList)
            {
                var template = Instantiate(lobbyTemplate, lobbyListContainer.transform);
                template.SetActive(true);
                template.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
            }
        }

        private void KitchenGameLobby_OnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
        {
            UpdateLobbyList(e.LobbyList);
        }
    }
}