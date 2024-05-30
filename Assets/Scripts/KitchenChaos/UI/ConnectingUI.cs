using KitchenChaos.Network;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class ConnectingUI : MonoBehaviour
    {

        private void Start()
        {
            KitchenGameMultiPlayer.Instance.OnTryingJoinGame += KitchenGameMultiPlayer_OnTryingJoinGame;
            KitchenGameMultiPlayer.Instance.OnFailedJoinGame += KitchenGameMultiPlayer_OnFailedJoinGame;

            Hide();
        }

        private void KitchenGameMultiPlayer_OnTryingJoinGame(object sender, System.EventArgs e)
        {
            Show();
        }

        private void KitchenGameMultiPlayer_OnFailedJoinGame(object sender, System.EventArgs e)
        {
            Hide();
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);

        private void OnDestroy()
        {
            KitchenGameMultiPlayer.Instance.OnTryingJoinGame -= KitchenGameMultiPlayer_OnTryingJoinGame;
            KitchenGameMultiPlayer.Instance.OnFailedJoinGame -= KitchenGameMultiPlayer_OnFailedJoinGame;
        }
    }
}
