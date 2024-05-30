using System;
using System.Collections.Generic;
using KitchenChaos.Network;
using KitchenChaos.SO;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos
{
    public class PlateKitchenObject : KitchenObject
    {
        [SerializeField] private List<KitchenObjectSO> validKitchenObjectSoList;

        public List<KitchenObjectSO> KitchenObjectSoList { get; private set; }

        public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
        public class OnIngredientAddedEventArgs : EventArgs
        {
            public KitchenObjectSO kitchenObjectSo;
        }


        protected override void Awake()
        {
            base.Awake();
            KitchenObjectSoList = new List<KitchenObjectSO>();
        }

        public void AddIngredient(KitchenObjectSO kitchenObjectSo)
        {
            if (!validKitchenObjectSoList.Contains(kitchenObjectSo))
            {
                return;
            }
            if (KitchenObjectSoList.Contains(kitchenObjectSo))
            {
                return;
            }
            var index = KitchenGameMultiPlayer.Instance.GetKitchenObjectIndex(kitchenObjectSo);
            AddIngredientServerRpc(index);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddIngredientServerRpc(int kitchenObjectSoIndex)
        {
            AddIngredientClientRpc(kitchenObjectSoIndex);
        }
        [ClientRpc]
        private void AddIngredientClientRpc(int kitchenObjectSoIndex)
        {
            var kitchenObjectSo = KitchenGameMultiPlayer.Instance.GetKitchenObjectSOByIndex(kitchenObjectSoIndex);
            KitchenObjectSoList.Add(kitchenObjectSo);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSo = kitchenObjectSo
            });
        }


        public bool TryAddIngredient(KitchenObjectSO kitchenObjectSo)
        {
            if (!validKitchenObjectSoList.Contains(kitchenObjectSo))
            {
                return false;
            }
            if (KitchenObjectSoList.Contains(kitchenObjectSo))
            {
                return false;
            }
            var index = KitchenGameMultiPlayer.Instance.GetKitchenObjectIndex(kitchenObjectSo);
            AddIngredientServerRpc(index);
            return true;
        }
    }
}