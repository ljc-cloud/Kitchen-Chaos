using System;
using System.Linq;
using UnityEngine;
using KitchenChaos.Player;


namespace KitchenChaos.Counter
{
    public class SelectedCounterVisual : MonoBehaviour
    {
        [SerializeField] private BaseCounter counter;
        [SerializeField] private GameObject[] selectedCounterVisuals;
        private void Start()
        {
            if (PlayerControl.LocalInstance != null)
            {
                PlayerControl.LocalInstance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
            }
            else
            {
                PlayerControl.OnAnyPlayerSpawned += PlayerControlOnOnAnyPlayerSpawned;
            }
        }

        private void PlayerControlOnOnAnyPlayerSpawned(object sender, EventArgs e)
        {
            if (PlayerControl.LocalInstance != null)
            {
                PlayerControl.LocalInstance.OnSelectedCounterChanged -= PlayerOnSelectedCounterChanged;
                PlayerControl.LocalInstance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
            }
        }

        private void OnDestroy()
        {
            PlayerControl.LocalInstance.OnSelectedCounterChanged -= PlayerOnSelectedCounterChanged;
            PlayerControl.OnAnyPlayerSpawned -= PlayerControlOnOnAnyPlayerSpawned;
        }

        private void PlayerOnSelectedCounterChanged(object sender, PlayerControl.OnSelectedCounterChangedEventArgs e)
        {
            if (counter == e.SelectedCounter)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show() => selectedCounterVisuals.ToList().ForEach(item => item.SetActive(true));
        private void Hide() => selectedCounterVisuals.ToList().ForEach(item => item.SetActive(false));
    }
}
