using KitchenChaos.Network;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class LobbyListSingleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;

        private Lobby _lobby;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(async() => 
            {
                await KitchenGameLobby.Instance.JoinLobbyById(_lobby.Id);
            });
        }

        public void SetLobby(Lobby lobby)
        { 
            _lobby = lobby;
            lobbyNameText.text = _lobby.Name;
        }
    }
}