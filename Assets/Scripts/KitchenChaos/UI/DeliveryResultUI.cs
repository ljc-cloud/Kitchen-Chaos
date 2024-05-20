using KitchenChaos.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class DeliveryResultUI : MonoBehaviour
    {
        private readonly int PopupHash = Animator.StringToHash("Popup");

        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Color successColor;
        [SerializeField] private Color failColor;
        [SerializeField] private Sprite successSprite;
        [SerializeField] private Sprite failSprite;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFail += DeliveryManager_OnRecipeFail;
            gameObject.SetActive(false);
        }

        private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
        {
            Debug.Log("Play Correct Anim");
            gameObject.SetActive(true);
            backgroundImage.color = successColor;
            iconImage.sprite = successSprite;
            messageText.text = "Delivery\nSuccess";
            _animator.SetTrigger(PopupHash);
        }
        private void DeliveryManager_OnRecipeFail(object sender, System.EventArgs e)
        {
            Debug.Log("Play InCorrect Anim");
            gameObject.SetActive(true);
            backgroundImage.color = failColor;
            iconImage.sprite = failSprite;
            messageText.text = "Delivery\nFailed";
            _animator.SetTrigger(PopupHash);
        }
    }

}

