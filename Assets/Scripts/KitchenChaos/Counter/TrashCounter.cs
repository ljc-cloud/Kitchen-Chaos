using KitchenChaos.Player;
using System;

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
                player.KitchenObj.DestroySelf();
                OnDropSomething?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}