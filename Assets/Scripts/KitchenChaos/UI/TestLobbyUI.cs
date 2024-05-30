using KitchenChaos.Network;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class TestLobbyUI : MonoBehaviour
    {
        [SerializeField] private Button createGameButton;
        [SerializeField] private Button joinGameButton;

        private void Awake()
        {
            createGameButton.onClick.AddListener(() =>
            {
                KitchenGameMultiPlayer.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            });
            joinGameButton.onClick.AddListener(() =>
            {
                KitchenGameMultiPlayer.Instance.StartClient();
            });
        }
    }
}