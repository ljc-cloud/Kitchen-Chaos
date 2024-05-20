using System;
using KitchenChaos.Counter;
using KitchenChaos.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class ProgressUI : MonoBehaviour
    {
        [SerializeField] private GameObject hasProgressGameObject;
        [SerializeField] private Image progressbar;

        private IHasProgress _hasProgress;

        private void Start()
        {
            if (hasProgressGameObject.TryGetComponent(out IHasProgress hasProgress))
            {
                _hasProgress = hasProgress;
            }
            else
            {
                Debug.LogError($"hasProgressGameObject Does Not Implement Interface IHasProgress");
                return;
            }
            _hasProgress.OnProgressbarChanged += CuttingCounterOnOnProgressbarChanged;
            progressbar.fillAmount = 0f;
            Hide();
        }

        private void CuttingCounterOnOnProgressbarChanged(object sender, IHasProgress.OnProgressbarChangedEventArgs e)
        {
            if (e.progressNormalized == 0f || Math.Abs(e.progressNormalized - 1f) < 0.01f)
            {
                Hide();
            }
            else
            {
                Show();
            }
            progressbar.fillAmount = e.progressNormalized;
            
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}