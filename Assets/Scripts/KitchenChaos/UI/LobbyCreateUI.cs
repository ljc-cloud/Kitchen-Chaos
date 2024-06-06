using KitchenChaos.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class LobbyCreateUI : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button createPublicButton;
        [SerializeField] private Button createPrivateButton;
        [SerializeField] private TMP_InputField lobbyNameInputField;

        private void Awake()
        {
            closeButton.onClick.AddListener(() =>
            {
                Hide();
            });
            createPublicButton.onClick.AddListener(async () =>
            {
                await KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
            });
            createPrivateButton.onClick.AddListener(async () =>
            {
                await KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
            });
            Hide();
        }

        public void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}