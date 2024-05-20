using UnityEngine;
using KitchenChaos.Manager;

namespace KitchenChaos.Player
{
    public class PlayerSound : MonoBehaviour
    {
        private PlayerControl _playerControl;
        private float _footstepTimer;
        private float _footstepTimerMax = .1f;

        private void Awake()
        {
            _playerControl = GetComponent<PlayerControl>();
        }

        private void Update()
        {
            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer < 0)
            {
                _footstepTimer = _footstepTimerMax;
                if (_playerControl.IsWalking)
                {
                    var volume = 1f;
                    SoundManager.Instance.PlayFootstepSound(transform.position, volume);
                }
            }
        }
    }
}