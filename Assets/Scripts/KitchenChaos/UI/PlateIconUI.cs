using System;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class PlateIconUI : MonoBehaviour
    {
        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private GameObject iconTemplate;

        private void Awake()
        {
            iconTemplate.SetActive(false);
        }

        private void Start()
        {
            plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnOnIngredientAdded;
        }

        private void PlateKitchenObjectOnOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
        {
            var iconTemplateGameObject = Instantiate(iconTemplate, transform);
            iconTemplateGameObject.SetActive(true);
            iconTemplateGameObject.GetComponent<IconTemplate>().UpdateImage(e.kitchenObjectSo.sprite);
        }
    }
}

