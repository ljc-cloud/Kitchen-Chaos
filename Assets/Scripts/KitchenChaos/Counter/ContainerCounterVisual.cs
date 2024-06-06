using System;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class ContainerCounterVisual : MonoBehaviour
    {
        private Animator _animator;

        [SerializeField] private ContainerCounter containerCounter;
        private static readonly int OpenCloseHash = Animator.StringToHash("OpenClose");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            containerCounter.OnPlayerDragObject += ContainerCounterOnOnPlayerDragObject;
        }

        private void OnDestroy()
        {
            containerCounter.OnPlayerDragObject -= ContainerCounterOnOnPlayerDragObject;
        }

        private void ContainerCounterOnOnPlayerDragObject(object sender, EventArgs e)
        {
            _animator.SetTrigger(OpenCloseHash);
        }
    }
}