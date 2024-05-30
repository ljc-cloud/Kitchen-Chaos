using KitchenChaos.Counter;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class StoveCounterWarningUI : MonoBehaviour
    {
        private readonly int IsFlashHash = Animator.StringToHash("IsWarningFlash");
        private Animator _animator;

        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private Image warningImage;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            stoveCounter.OnProgressbarChanged += StoveCounter_OnProgressbarChanged;
            Hide();
        }

        private void StoveCounter_OnProgressbarChanged(object sender, Interface.IHasProgress.OnProgressbarChangedEventArgs e)
        {
            Show();
            bool playAnim = stoveCounter.IsFried && e.progressNormalized > .5f;
            warningImage.gameObject.SetActive(playAnim);
            _animator.SetBool(IsFlashHash, playAnim);
        }
        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}

