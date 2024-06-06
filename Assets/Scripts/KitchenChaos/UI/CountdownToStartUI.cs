using KitchenChaos.Manager;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class CountdownToStartUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countdownText;

        private void Awake()
        {
            
        }

        private void Start()
        {
            GameManager.Instance.OnStateChanged += GamaManagerOnStateChanged;
            Hide();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnStateChanged -= GamaManagerOnStateChanged;
        }

        private void GamaManagerOnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsCountdownToStart)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Update()
        {
            if (GameManager.Instance.IsCountdownToStart)
            {
                countdownText.text = Mathf.Ceil(GameManager.Instance.CountdownToStartTimer).ToString();
            }
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}
