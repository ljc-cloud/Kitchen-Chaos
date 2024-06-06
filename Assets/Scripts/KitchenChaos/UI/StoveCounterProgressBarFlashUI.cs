using KitchenChaos.Counter;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class StoveCounterProgressBarFlashUI : MonoBehaviour
    {
        private readonly int IsFlashHash = Animator.StringToHash("IsProgressFlash");

        [SerializeField] private StoveCounter stoveCounter;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }   

        private void Start()
        {
            stoveCounter.OnProgressbarChanged += StoveCounter_OnProgressbarChanged;
        }

        private void OnDestroy()
        {
            stoveCounter.OnProgressbarChanged -= StoveCounter_OnProgressbarChanged;
        }

        private void StoveCounter_OnProgressbarChanged(object sender, Interface.IHasProgress.OnProgressbarChangedEventArgs e)
        {
            bool playAnim = stoveCounter.IsFried && e.progressNormalized > .5f;
            _animator.SetBool(IsFlashHash, playAnim);

        }
    }
}
