using KitchenChaos.Network;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class CharacterColorSelectSingleUI : MonoBehaviour
    {
        [SerializeField] private int colorId;
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedGameObject;

        private Button _colorSelectButton;

        private void Awake()
        {
            _colorSelectButton = GetComponent<Button>();
            _colorSelectButton.onClick.AddListener(() =>
            {
                KitchenGameMultiPlayer.Instance.ChangePlayerColor(colorId);
            });
            
        }

        private void Start()
        {
            KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += NetworkManager_OnPlayerDataNetworkListChanged;
            image.color = KitchenGameMultiPlayer.Instance.GetPlayerColorByIndex(colorId);
            SetIsSelected(!KitchenGameMultiPlayer.Instance.IsColorAvalable(colorId));
        }

        private void NetworkManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
        {
            //print($"Set Color {colorId} Avalable {KitchenGameMultiPlayer.Instance.IsColorAvalable(colorId)}");
            SetIsSelected(!KitchenGameMultiPlayer.Instance.IsColorAvalable(colorId));
        }

        private void SetIsSelected(bool selected)
        {
            selectedGameObject.SetActive(selected);
            _colorSelectButton.enabled = !selected;
        }

        private void OnDestroy()
        {
            KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged -= NetworkManager_OnPlayerDataNetworkListChanged;
        }
    }
}