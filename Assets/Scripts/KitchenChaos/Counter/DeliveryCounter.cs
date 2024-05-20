using KitchenChaos.Player;
using KitchenChaos.Manager;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class DeliveryCounter : BaseCounter
    {
        public static DeliveryCounter Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public override void Interact(PlayerControl player)
        {
            Debug.Log($"DeliveryCounter.Interact Invoked, Player HasKitchenObject:{player.HasKitchenObject}");
            if (player.HasKitchenObject)
            {
                if (player.KitchenObj.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                {
                    DeliveryManager.Instance.DeliveryRecipe(plateKitchenObject);
                    player.KitchenObj.DestroySelf();
                }
            }
        }
    }

}
