using KitchenChaos.Manager;
using KitchenChaos.Player;
using TMPro;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moveUpKeyText;
        [SerializeField] private TextMeshProUGUI moveDownKeyText;
        [SerializeField] private TextMeshProUGUI moveLeftKeyText;
        [SerializeField] private TextMeshProUGUI moveRightKeyText;
        [SerializeField] private TextMeshProUGUI interactKeyText;
        [SerializeField] private TextMeshProUGUI interactAltKeyText;
        [SerializeField] private TextMeshProUGUI pauseKeyText;

        private void Start()
        {
            GameManager.Instance.OnTutorialEnd += TutorialUIOnTutorialEnd;

            moveUpKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveUp);
            moveDownKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveDown);
            moveLeftKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveLeft);
            moveRightKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.MoveRight);
            interactKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.Interact);
            interactAltKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.InteractAlt);
            pauseKeyText.text = GameInput.Instance.GetKeyBindingString(GameInput.KeyBinding.Pause);
        }

        private void TutorialUIOnTutorialEnd(object sender, System.EventArgs e)
        {
            Hide();
        }

        private void Hide() => gameObject.SetActive(false);
    }

}
