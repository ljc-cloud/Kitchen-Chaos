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

        /// <summary>
        /// 投递已经做好的Recipe
        /// </summary>
        /// <param name="player"></param>
        public override void Interact(PlayerControl player)
        {
            if (player.HasKitchenObject)
            {
                if (player.KitchenObj.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                {
                    DeliveryManager.Instance.DeliveryRecipe(plateKitchenObject);
                    KitchenObject.DestroyKitchenObject(player.KitchenObj);
                }
            }
        }
    }

}

