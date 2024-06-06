using System;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class CuttingCounterVisual : MonoBehaviour
    {
        [SerializeField] private CuttingCounter cuttingCounter;
        private Animator _animator;

        private readonly int _cutHash = Animator.StringToHash("Cut");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            cuttingCounter.OnCut += CuttingCounterOnOnCut;
        }

        private void OnDestroy()
        {
            cuttingCounter.OnCut -= CuttingCounterOnOnCut;
        }

        private void CuttingCounterOnOnCut(object sender, EventArgs e)
        {
            _animator.SetTrigger(_cutHash);
        }
    }
}