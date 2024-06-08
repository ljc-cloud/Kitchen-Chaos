using KitchenChaos.Network;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class KickMessageUI : MonoBehaviour
    {
        [SerializeField] private Button playAgainButton;

        private void Awake()
        {
            playAgainButton.onClick.AddListener(() => Loader.Load(Loader.Scene.MainMenuScene));
        }

        private void Start()
        {
            KitchenGameMultiPlayer.Instance.OnKickPlayer += KitchenGameMultiPlayer_OnKickPlayer;
            Hide();
        }

        private void KitchenGameMultiPlayer_OnKickPlayer(object sender, System.EventArgs e)
        {
            Show();
        }

        private void Show()=> gameObject.SetActive(true);
        private void Hide()=> gameObject.SetActive(false);
    }
}