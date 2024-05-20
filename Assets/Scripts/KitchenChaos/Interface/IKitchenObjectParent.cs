using KitchenChaos.SO;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Interface
{
    public interface IKitchenObjectParent
    {
        KitchenObject KitchenObj { get; set; }
        Transform KitchenObjectFollowTransform { get; }
        bool HasKitchenObject { get; }
        void ClearKitchenObject();
        NetworkObject GetNetworkObject();
    }
}