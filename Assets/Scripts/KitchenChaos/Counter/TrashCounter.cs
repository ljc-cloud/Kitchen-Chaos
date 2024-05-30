using KitchenChaos.Player;
using System;
using Unity.Netcode;

namespace KitchenChaos.Counter
{
    public class TrashCounter : BaseCounter
    {

        public static event EventHandler OnDropSomething;

        public static new void ResetStaticData() {
            OnDropSomething = null;
        }

        public override void Interact(PlayerControl player)
        {
            if (player.HasKitchenObject)
            {
                KitchenObject.DestroyKitchenObject(player.KitchenObj);
                InteractLogicServerRpc();
                //player.KitchenObj.DestroySelf();
                //OnDropSomething?.Invoke(this, EventArgs.Empty);
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
            OnDropSomething?.Invoke(this, EventArgs.Empty);
        }
    }
}