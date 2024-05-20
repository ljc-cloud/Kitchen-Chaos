using KitchenChaos.Counter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class StoveCounterWarningUI : MonoBehaviour
    {
        private readonly int IsFlashHash = Animator.StringToHash("IsWarningFlash");
        private Animator _animator;

        [SerializeField] private StoveCounter stoveCounter;

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
            _animator.SetBool(IsFlashHash, playAnim);
        }
        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}

