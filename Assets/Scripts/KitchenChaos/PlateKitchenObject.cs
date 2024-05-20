using System;
using System.Collections.Generic;
using KitchenChaos.SO;
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


        private void Awake()
        {
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
            KitchenObjectSoList.Add(kitchenObjectSo);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSo = kitchenObjectSo
            });
            return true;
        }
    }
}