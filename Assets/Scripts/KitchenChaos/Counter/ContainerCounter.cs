using System;
using KitchenChaos.Player;
using KitchenChaos.SO;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class ContainerCounter : BaseCounter
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSo;

        public event EventHandler OnPlayerDragObject;

        public override void Interact(PlayerControl player)
        {

            if (player.HasKitchenObject)
            {
                if (player.KitchenObj.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                {
                    plateKitchenObject.TryAddIngredient(kitchenObjectSo);
                }
            }
            else
            {
                KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
                OnPlayerDragObject?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}