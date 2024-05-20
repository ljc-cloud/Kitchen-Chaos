using Unity.Netcode;
using UnityEngine;


namespace KitchenChaos.Player
{
    public class PlayerAnimator : NetworkBehaviour
    {

        private Animator _animator;
        [SerializeField] private PlayerControl player;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        //[ServerRpc]
        //private void HandleAnimatorServerRpc()
        //{
        //    _animator.SetBool(IsWalking, player.IsWalking);
        //}

        private void Update()
        {
            if (!IsOwner) return;
            _animator.SetBool(IsWalking, player.IsWalking);
        }
    }

}
