using KitchenChaos.Manager;
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
            GameManager.Instance.OnGamePause += (sender, e) => Show();
            GameManager.Instance.OnGameUnPause += (sender, e) => Hide();

            Hide();
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }

}
