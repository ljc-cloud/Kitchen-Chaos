using KitchenChaos.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class GamePlayingClockUI : MonoBehaviour
    {
        [SerializeField] private Image gamePlayingClock;

        private void Update()
        {
            if (!GameManager.Instance.IsGamePlaying) return;
            gamePlayingClock.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
        }
    }
}