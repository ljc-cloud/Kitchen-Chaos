using System;
using System.Collections.Generic;
using KitchenChaos.SO;
using UnityEngine;

namespace KitchenChaos
{
    public class PlateCompleteVisual : MonoBehaviour
    {

        [Serializable]
        public struct KitchenObjectSO_GameObject
        {
            public KitchenObjectSO kitchenObjectSo;
            public GameObject gameObject;
        }

        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSoGameObjectList;

        private void Start()
        {
            plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnOnIngredientAdded;
            kitchenObjectSoGameObjectList.ForEach(item => item.gameObject.SetActive(false));
        }

        private void PlateKitchenObjectOnOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
        {
            var gameObjectVisual = kitchenObjectSoGameObjectList.Find(item => Equals(e.kitchenObjectSo, item.kitchenObjectSo)).gameObject;
            gameObjectVisual.SetActive(true);
        }
    }

}
