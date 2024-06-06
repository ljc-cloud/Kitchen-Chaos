using KitchenChaos.Manager;
using KitchenChaos.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class OptionsUI : MonoBehaviour
    {
        public static OptionsUI Instance { get; private set; }

        [SerializeField] private Button soundEffectButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI soundEffectText;
        [SerializeField] private TextMeshProUGUI musicText;

        // KeyBindings 
        [SerializeField] private TextMeshProUGUI MoveUpKeyText;
        [SerializeField] private TextMeshProUGUI MoveDownKeyText;
        [SerializeField] private TextMeshProUGUI MoveLeftKeyText;
        [SerializeField] private TextMeshProUGUI MoveRightKeyText;
        [SerializeField] private TextMeshProUGUI MoveInteractKeyText;
        [SerializeField] private TextMeshProUGUI MoveInteractAltKeyText;
        [SerializeField] private TextMeshProUGUI PauseKeyText;
        [SerializeField] private Button MoveUpBindingButton;
        [SerializeField] private Button MoveDownBindingButton;
        [SerializeField] private Button MoveLeftBindingButton;
        [SerializeField] private Button MoveRightBindingButton;
        [SerializeField] private Button InteractBindingButton;
        [SerializeField] private Button InteractAltBindingButton;
        [SerializeField] private Button PauseBindingButton;

        [SerializeField] private GameObject PressKeyToRebind;


        private Action _onOptionUIHide;

        private void Awake()
        {
            Instance = this;
            soundEffectButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.ChangeVolume();
                UpdateVisual();
            });
            musicButton.onClick.AddListener(() =>
            {
                MusicManager.Instance.ChangeVolume();
                UpdateVisual();
            });
            closeButton.onClick.AddListener(() =>
            {
                Hide();
            });

            // KeyBindings
            MoveUpBindingButton.onClick.AddListener(() => RebindBinding(GameInput.KeyBinding.MoveUp));
            MoveDownBindingButton.onClick.AddListener(() => RebindBinding(GameInput.KeyBinding.MoveDown));
            MoveLeftBindingButton.onClick.AddListener(() => RebindBinding(GameInput.KeyBinding.MoveLeft));
            MoveRightBindingButton.onClick.AddListener(() => RebindBinding(GameInput.KeyBinding.MoveRight));
            InteractBindingButton.onClick.AddListener(() => RebindBinding(GameInput.KeyBinding.Interact));
            InteractAltBindingButton.onClick.AddListener(() => RebindBinding(GameInput.KeyBinding.InteractAlt));
            PauseBindingButton.onClick.AddListener(() => RebindBinding(GameInput.KeyBinding.Pause));
        }

        private void RebindBinding(GameInput.KeyBinding binding)
        {
            ShowPressKeyToRebind();
            GameInput.Instance.RebindBinding(binding, () =>
            {
                HidePressKeyToRebind();
                UpdateVisual();
            });
        }


        private void Start()
        {
            GameManager.Instance.OnLocalGameUnpause += GameManager_OnLocalGameUnpause;
            UpdateVisual();
            Hide();
            HidePressKeyToRebind();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnLocalGameUnpause -= GameManager_OnLocalGameUnpause;
        }

        private void GameManager_OnLocalGameUnpause(object sender, EventArgs e)
        {
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            _onOptionUIHide?.Invoke();
        }
        public void Show(Action onOptionUIHide)
        {
            _onOptionUIHide = onOptionUIHide;
            gameObject.SetActive(true);
        }

        private void UpdateVisual()
        {
            soundEffectText.text = $"SoundEffect:{Mathf.Round(SoundManager.Instance.GetVolume() * 10f)}";
            musicText.text = $"Music:{Mathf.Round(MusicManager.Instance.GetVolume() * 10f)}";

            // KeyBindings 
            MoveUpKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveUp);
            MoveDownKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveDown);
            MoveLeftKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveLeft);
            MoveRightKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveRight);
            MoveInteractKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.Interact);
            MoveInteractAltKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.InteractAlt);
            PauseKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.Pause);
        }

        private void ShowPressKeyToRebind()
        {
            PressKeyToRebind.SetActive(true);
        }
        private void HidePressKeyToRebind()
        {
            PressKeyToRebind.SetActive(false);
        }
    }
}

