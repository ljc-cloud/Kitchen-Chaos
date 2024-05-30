using KitchenChaos.Manager;
using KitchenChaos.Player;
using KitchenChaos.SO;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class ClearCounter : BaseCounter
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSo;

        public override void Interact(PlayerControl player)
        {
            if (GameManager.Instance.IsGamePause)
            {
                return;
            }
            if (!HasKitchenObject)
            {
                if (!player.HasKitchenObject)
                {
                }
                else
                {
                    player.KitchenObj.SetKitchenObjectParent(this);
                }
            }
            else
            {
                if (!player.HasKitchenObject)
                {
                    KitchenObj.SetKitchenObjectParent(player);
                }
                else
                {
                    if (player.KitchenObj.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(KitchenObj.KitchenObjectSo))
                        {
                            KitchenObject.DestroyKitchenObject(KitchenObj);
                        }
                    }
                    else
                    {
                        if (KitchenObj.TryGetPlateKitchenObject(out plateKitchenObject))
                        {
                            if (plateKitchenObject.TryAddIngredient(player.KitchenObj.KitchenObjectSo))
                            {
                                KitchenObject.DestroyKitchenObject(player.KitchenObj);
                            }
                        }
                    }
                }
            }
        }
    }
}