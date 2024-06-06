using KitchenChaos.Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class GamePauseUI : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button optionsButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(() =>
            {
                GameManager.Instance.ToogleGamePause();
                Hide();
            });
            mainMenuButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MainMenuScene);
            });
            optionsButton.onClick.AddListener(() =>
            {
                Hide();
                OptionsUI.Instance.Show(Show);
            });
        }

        private void Start()
        {
            GameManager.Instance.OnLocalGamePause += GameManager_OnLocalGamePause;
            GameManager.Instance.OnLocalGameUnpause += GameManager_OnLocalGameUnpause;

            Hide();
        }

        private void GameManager_OnLocalGameUnpause(object sender, System.EventArgs e)
        {
            Hide();
        }

        private void GameManager_OnLocalGamePause(object sender, System.EventArgs e)
        {
            Show();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnLocalGamePause -= GameManager_OnLocalGamePause;
            GameManager.Instance.OnLocalGameUnpause -= GameManager_OnLocalGameUnpause;
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }

}
