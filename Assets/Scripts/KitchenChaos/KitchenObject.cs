using KitchenChaos.Interface;
using KitchenChaos.Network;
using KitchenChaos.SO;
using UnityEngine;
using Unity.Netcode;

namespace KitchenChaos
{
    public class KitchenObject : NetworkBehaviour
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSo;

        public IKitchenObjectParent KitchenObjectParent { get; private set; }

        public KitchenObjectSO KitchenObjectSo => kitchenObjectSo;

        public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
        {
            KitchenGameMultiPlayer.Instance.SpawnKitchenObject(kitchenObjectSo, kitchenObjectParent);
            //var kitchenObjectGameObject = Instantiate(kitchenObjectSo.prefab);
            //var kitchenObject = kitchenObjectGameObject.GetComponent<KitchenObject>();
            //kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
            //return kitchenObject;
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

            //transform.parent = KitchenObjectParent.KitchenObjectFollowTransform;
            //transform.localPosition = Vector3.zero;
        }
        
        public void DestroySelf()
        {
            KitchenObjectParent.ClearKitchenObject();
            Destroy(gameObject);
        }
    }
}

