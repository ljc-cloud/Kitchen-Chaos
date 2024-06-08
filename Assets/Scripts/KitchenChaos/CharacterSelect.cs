using KitchenChaos.Network;
using KitchenChaos.Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos
{
    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField] private int playerIndex;
        [SerializeField] private GameObject readyTextGameObject;
        [SerializeField] private PlayerVisual playerVisual;
        [SerializeField] private Button kickButton;
        [SerializeField] private TextMeshPro playerNameText;

        private void Awake()
        {
            kickButton.onClick.AddListener(() =>
            {
                var playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataByPlayerIndex(playerIndex);
                KitchenGameLobby.Instance.KickPlayer(playerData.PlayerId.ToString());
                KitchenGameMultiPlayer.Instance.KickPlayer(playerData.ClientId);
            });
        }

        private void Start()
        {
            KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged;
            CharacterSelectReady.Instance.OnPlayerReadyChanged += CharacterSelectReady_OnPlayerReadyChanged;
            UpdateCharacterVisual();
            UpdatePlayerReady();
            kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer &&
                !KitchenGameMultiPlayer.Instance.PlayerIndexIsServer(playerIndex));
        }

        private void OnDestroy()
        {
            KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged;
            CharacterSelectReady.Instance.OnPlayerReadyChanged -= CharacterSelectReady_OnPlayerReadyChanged;
        }

        private void CharacterSelectReady_OnPlayerReadyChanged(object sender, System.EventArgs e)
        {
            UpdatePlayerReady();
        }

        private void KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
        {
            UpdateCharacterVisual();
        }

        private void UpdateCharacterVisual()
        {
            if (KitchenGameMultiPlayer.Instance.IsPlayerIndexConnected(playerIndex))
            {
                var playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataByPlayerIndex(playerIndex);
                var color = KitchenGameMultiPlayer.Instance.GetPlayerColorByIndex(playerData.ColorId);
                playerVisual.SetPlayerColor(color);
                playerNameText.text = playerData.PlayerName.ToString();
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void UpdatePlayerReady()
        {
            if (KitchenGameMultiPlayer.Instance.IsPlayerIndexConnected(playerIndex))
            {
                var playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataByPlayerIndex(playerIndex);
                readyTextGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientId));
            }
            else
            {
                readyTextGameObject.SetActive(false);
            }
        }

        private void UpdateCharacterColor()
        {
            if (KitchenGameMultiPlayer.Instance.IsPlayerIndexConnected(playerIndex))
            {
                var playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataByPlayerIndex(playerIndex);
                var color = KitchenGameMultiPlayer.Instance.GetPlayerColorByIndex(playerData.ColorId);
                playerVisual.SetPlayerColor(color);
            }
        }


        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }

}
