using System;
using KitchenChaos.Manager;
using KitchenChaos.Player;
using KitchenChaos.SO;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Counter
{
    /// <summary>
    /// ContainerCounter
    /// </summary>
    public class ContainerCounter : BaseCounter
    {
        // Container
        [SerializeField] private KitchenObjectSO kitchenObjectSo;

        public event EventHandler OnPlayerDragObject;

        public override void Interact(PlayerControl player)
        {
            if (GameManager.Instance.IsGamePause)
            {
                return;
            }
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
                InteractLogicServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }

        [ClientRpc]
        private void InteractLogicClientRpc()
        {
            OnPlayerDragObject?.Invoke(this, EventArgs.Empty);
        }
    }
}