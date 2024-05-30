using KitchenChaos.Interface;
using KitchenChaos.Network;
using KitchenChaos.SO;
using UnityEngine;
using Unity.Netcode;
using IKitchenObjectParent = KitchenChaos.Interface.IKitchenObjectParent;

namespace KitchenChaos
{
    public class KitchenObject : NetworkBehaviour
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSo;

        public IKitchenObjectParent KitchenObjectParent { get; private set; }

        public KitchenObjectSO KitchenObjectSo => kitchenObjectSo;

        private FollowTransform _followTransform;
        protected virtual void Awake()
        {
            _followTransform = GetComponent<FollowTransform>();
        }

        public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
        {
            KitchenGameMultiPlayer.Instance.SpawnKitchenObject(kitchenObjectSo, kitchenObjectParent);
            //var kitchenObjectGameObject = Instantiate(kitchenObjectSo.prefab);
            //var kitchenObject = kitchenObjectGameObject.GetComponent<KitchenObject>();
            //kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
            //return kitchenObject;
        }
        public static void DestroyKitchenObject(KitchenObject kitchenObject)
        {
            KitchenGameMultiPlayer.Instance.DestroyKitchenObject(kitchenObject);
        }


        public bool TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject)
        {
            if (this is PlateKitchenObject)
            {
                plateKitchenObject = this as PlateKitchenObject;
                return true;
            }
            plateKitchenObject = null;
            return false;
        }

        public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
        {
            SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkReference)
        {
            SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkReference);
        }
        [ClientRpc]
        private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkReference)
        {
            kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            if (KitchenObjectParent != null)
            {
                KitchenObjectParent.ClearKitchenObject();
            }

            KitchenObjectParent = kitchenObjectParent;
            if (KitchenObjectParent.HasKitchenObject)
            {
                Debug.LogError("KitchenObjectParent already has KitchenObject");
            }
            KitchenObjectParent.KitchenObj = this;

            _followTransform.SetTargetTransform(kitchenObjectParent.KitchenObjectFollowTransform);
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
        public void ClearKitchenObjectOnParent()
        {
            KitchenObjectParent.ClearKitchenObject();
        }

    }
}

