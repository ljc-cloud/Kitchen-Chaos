using UnityEngine;

namespace KitchenChaos.Counter
{
    public class StoveCounterVisual : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private GameObject stoveOnGameObject;
        [SerializeField] private GameObject particleObject;

        private void Start()
        {
            stoveCounter.OnStateChanged += StoveCounterOnOnStateChanged;
        }

        private void OnDestroy()
        {
            stoveCounter.OnStateChanged -= StoveCounterOnOnStateChanged;
        }

        private void StoveCounterOnOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            bool show = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
            stoveOnGameObject.SetActive(show);
            particleObject.SetActive(show);
        }
    }
}