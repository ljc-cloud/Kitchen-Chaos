using KitchenChaos.Counter;
using KitchenChaos.Player;
using KitchenChaos.SO;
using Unity.VisualScripting;
using UnityEngine;
namespace KitchenChaos.Manager
{
    public class SoundManager : MonoBehaviour
    {
        private const string PlayerPrefsSoundEffectVolume = "SoundEffect:";
        public static SoundManager Instance { get; private set; }
        [SerializeField] private AudioClipRefsSO audioClipRefsSo;

        private void Awake()
        {
            Instance = this;

            var volume = PlayerPrefs.GetFloat(PlayerPrefsSoundEffectVolume, .9f);
            _volume = volume;
        }

        private float _volume = .9f;

        public void ChangeVolume()
        {
            _volume += .1f;
            if (_volume > 1f) _volume = 0;
            PlayerPrefs.SetFloat(PlayerPrefsSoundEffectVolume, _volume);
            PlayerPrefs.Save();
        }

        public float GetVolume() => _volume;

        private void Start()
        {
            DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerOnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFail += DeliveryManagerOnRecipeFail;
            PlayerControl.OnAnyPickUpSomething += PlayerOnPickupSomething;
            //PlayerControl.Instance.OnPickupSomething += PlayerOnPickupSomething;
            CuttingCounter.OnAnyCut += CuttingCounterOnAnyCut;
            BaseCounter.OnSomethingPlaced += BaseCounterOnSomethingPlaced;
            TrashCounter.OnDropSomething += TrashCounterOnDropSomething;
        }

        private void TrashCounterOnDropSomething(object sender, System.EventArgs e)
        {
            TrashCounter trashCounter = sender as TrashCounter;
            PlaySound(audioClipRefsSo.objectDrop, trashCounter.transform.position);
        }

        private void BaseCounterOnSomethingPlaced(object sender, System.EventArgs e)
        {
            BaseCounter baseCounter = sender as BaseCounter;
            PlaySound(audioClipRefsSo.objectDrop, baseCounter.transform.position);
        }

        private void CuttingCounterOnAnyCut(object sender, System.EventArgs e)
        {
            CuttingCounter cuttingCounter = sender as CuttingCounter;
            PlaySound(audioClipRefsSo.chop, cuttingCounter.transform.position);
        }

        private void PlayerOnPickupSomething(object sender, System.EventArgs e)
        {
            var player = sender as PlayerControl;
            PlaySound(audioClipRefsSo.objectPickup, player.transform.position);
        }

        private void DeliveryManagerOnRecipeSuccess(object sender, System.EventArgs e)
        {
            Debug.Log("Play Correct Sound");
            PlaySound(audioClipRefsSo.deliverySuccess, DeliveryCounter.Instance.transform.position);
        }
        private void DeliveryManagerOnRecipeFail(object sender, System.EventArgs e)
        {
            Debug.Log("Play InCorrect Sound");
            PlaySound(audioClipRefsSo.deliveryFail, DeliveryCounter.Instance.transform.position);
        }

        private void PlaySound(AudioClip[] audioClipArr, Vector3 postion, float volumeMultiplier = 1f)
        {
            PlaySound(audioClipArr[Random.Range(0, audioClipArr.Length)], postion, _volume * volumeMultiplier);
        }

        private void PlaySound(AudioClip audioClip, Vector3 postion, float volumeMultiplier = 1f)
        {
            AudioSource.PlayClipAtPoint(audioClip, postion, _volume * volumeMultiplier);
        }
        public void PlayFootstepSound(Vector3 position, float volumeMultiplier = 1f)
        {
            PlaySound(audioClipRefsSo.footstep, position, _volume * volumeMultiplier);
        }
        public void PlayStoveCounterWarningSound(Vector3 position, float volumeMultiplier = 1f)
        {
            PlaySound(audioClipRefsSo.warning[0], position, _volume * volumeMultiplier);
        }
    }
}