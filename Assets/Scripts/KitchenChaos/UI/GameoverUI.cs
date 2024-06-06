using UnityEngine;
using KitchenChaos.Manager;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

namespace KitchenChaos.UI
{
    public class GameoverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI deliveredSuccessNumText;
        [SerializeField] private Button playAgainButton;

        private void Start()
        {
            GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;

            playAgainButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MainMenuScene);
            });

            Hide();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnStateChanged -= GameManagerOnStateChanged;
        }

        private void GameManagerOnStateChanged(object sender, System.EventArgs e)
        {
            if (GameManager.Instance.IsGameover)
            {
                Show();
                deliveredSuccessNumText.text = DeliveryManager.Instance.DeliverySuccessAmount.ToString();
            }
            else
            {
                Hide();
            }
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}
