using KitchenChaos.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class WaitingPlayersUI : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.OnLocalPlayerReady += GameManager_OnLocalPlayerReady;
            GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
            Hide();
        }

        private void GameManager_OnStateChanged(object sender, System.EventArgs e)
        {
            if (GameManager.Instance.IsCountdownToStart)
            {
                Hide();
            }
        }

        private void GameManager_OnLocalPlayerReady(object sender, System.EventArgs e)
        {
            if (GameManager.Instance.IsLocalPlayerReady)
            {
                Show();
            }
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}

