using UnityEngine;
using KitchenChaos.Manager;
using TMPro;

namespace KitchenChaos.UI
{
    public class GameoverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI deliveredSuccessNumText;

        private void Start()
        {
            GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
            Hide();
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
