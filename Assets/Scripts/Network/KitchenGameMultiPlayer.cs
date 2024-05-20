using KitchenChaos.Interface;
using KitchenChaos.SO;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Network
{
    public class KitchenGameMultiPlayer : NetworkBehaviour
    {
        public static KitchenGameMultiPlayer Instance { get; private set; }

        [SerializeField] private KitchenObjectListSO kitchenObjectListSo;

        private void Awake()
        {
            Instance = this;
        }

        public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
        {
            var index = GetKitchenObjectIndex(kitchenObjectSo);
            SpawnKitchenObjectServerRpc(index, kitchenObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex, NetworkObjectReference kitchenObjectParentNetworkReference)
        {
            var kitchenObjectSo = GetKitchenObjectSOByIndex(kitchenObjectSoIndex);
            var kitchenObjectGameObject = Instantiate(kitchenObjectSo.prefab);
            var kitchenObject = kitchenObjectGameObject.GetComponent<KitchenObject>();
            kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            kitchenObjectParentNetworkObject.Spawn(true);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }

        private int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSo)
        {
            return kitchenObjectListSo.kitchenObjectSoList.IndexOf(kitchenObjectSo);
        }

        private KitchenObjectSO GetKitchenObjectSOByIndex(int index)
        {
            return kitchenObjectListSo.kitchenObjectSoList[index];
        }
    }

}
