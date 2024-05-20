using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KitchenChaos.Player
{
    public class GameInput : MonoBehaviour
    {
        private const string PlayerPrefsBindingKey = "bindings:";
        public static GameInput Instance { get; private set; }

        public event EventHandler OnInteractAction;
        public event EventHandler OnInteractAlternateAction;
        public event EventHandler OnGamePauseAction;

        private PlayerInputActions _playerInputActions;


        private void Awake()
        {
            Instance = this;
            _playerInputActions = new PlayerInputActions();
            if (PlayerPrefs.HasKey(PlayerPrefsBindingKey))
            {
                var bindingJson = PlayerPrefs.GetString(PlayerPrefsBindingKey);
                _playerInputActions.LoadBindingOverridesFromJson(bindingJson);
            }
            _playerInputActions.Enable();

            _playerInputActions.Player.Interact.performed += InteractPerform;
            _playerInputActions.Player.InteractAlternate.performed += InteractAlternatePerformed;
            _playerInputActions.Player.PauseGame.performed += PauseGamePerformed;
        }

        private void OnDestroy()
        {
            _playerInputActions.Player.Interact.performed -= InteractPerform;
            _playerInputActions.Player.InteractAlternate.performed -= InteractAlternatePerformed;
            _playerInputActions.Player.PauseGame.performed -= PauseGamePerformed;

            _playerInputActions.Dispose();
        }

        public enum KeyBinding
        {
            MoveUp,
            MoveDown,
            MoveLeft,
            MoveRight,
            Interact,
            InteractAlt,
            Pause,
        }

        public string GetKeyBindingString(KeyBinding binding)
        {
            switch (binding)
            {
                case KeyBinding.MoveUp:
                    return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
                case KeyBinding.MoveDown:
                    return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
                case KeyBinding.MoveLeft:
                    return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
                case KeyBinding.MoveRight:
                    return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
                case KeyBinding.Interact:
                    return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
                case KeyBinding.InteractAlt:
                    return _playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
                case KeyBinding.Pause:
                    return _playerInputActions.Player.PauseGame.bindings[0].ToDisplayString();
                default: return null;
            }
        }

        public void RebindBinding(KeyBinding binding, Action onRebindedAction)
        {
            _playerInputActions.Player.Disable();
            InputAction inputAction;
            int index = 0;
            switch (binding)
            {
                case KeyBinding.MoveUp:
                    inputAction = _playerInputActions.Player.Move;
                    index = 1;
                    break;
                case KeyBinding.MoveDown:
                    inputAction = _playerInputActions.Player.Move;
                    index = 2;
                    break;
                case KeyBinding.MoveLeft:
                    inputAction = _playerInputActions.Player.Move;
                    index = 3;
                    break;
                case KeyBinding.MoveRight:
                    inputAction = _playerInputActions.Player.Move;
                    index = 4;
                    break;
                case KeyBinding.Interact:
                    inputAction = _playerInputActions.Player.Interact;
                    index = 0;
                    break;
                case KeyBinding.InteractAlt:
                    inputAction = _playerInputActions.Player.InteractAlternate;
                    index = 0;
                    break;
                case KeyBinding.Pause:
                    inputAction = _playerInputActions.Player.PauseGame;
                    index = 0;
                    break;
                default:
                    inputAction = null;
                    index = 0;
                    break;
            }
            inputAction?.PerformInteractiveRebinding(index)
            .OnComplete(callback =>
            {
                callback.Dispose();
                _playerInputActions.Player.Enable();
                onRebindedAction();
                PlayerPrefs.SetString(PlayerPrefsBindingKey, _playerInputActions.SaveBindingOverridesAsJson());
            }).Start();
        }

        private void PauseGamePerformed(InputAction.CallbackContext obj)
        {
            OnGamePauseAction?.Invoke(this, EventArgs.Empty);
        }

        private void InteractAlternatePerformed(InputAction.CallbackContext callbackContext)
        {
            OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
        }

        private void InteractPerform(InputAction.CallbackContext callbackContext)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }

        public Vector2 GetInputVectorNormalized()
        {
            return _playerInputActions.Player.Move.ReadValue<Vector2>();
        }
    }
}