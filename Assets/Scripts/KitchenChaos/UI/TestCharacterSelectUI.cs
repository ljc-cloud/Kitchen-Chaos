using KitchenChaos.Network;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class TestCharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private Button readyButton;

        private void Awake()
        {
            readyButton.onClick.AddListener(() =>
            {
                CharacterSelectReady.Instance.SetPlayerReady();
            });
        }
    }

}