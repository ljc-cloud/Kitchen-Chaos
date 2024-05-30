using KitchenChaos.Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetworkUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;


    private void Start()
    {
        hostButton.onClick.AddListener(() =>
        {
            KitchenGameMultiPlayer.Instance.StartHost();
            gameObject.SetActive(false);
        });

        clientButton.onClick.AddListener(() =>
        {
            KitchenGameMultiPlayer.Instance.StartClient();
            gameObject.SetActive(false);
        });
    }
}
