using KitchenChaos.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class RecipeTemplateUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recipeName;
        [SerializeField] private GameObject iconContainer;
        [SerializeField] private GameObject iconTemplate;


        private void Start()
        {
            iconTemplate.SetActive(false);
        }

        public void SetRecipeSo(RecipeSO recipeSo) 
        {
            recipeName.text = recipeSo.recipeName;

            foreach (Transform child in iconContainer.transform)
            {
                if (child == iconTemplate.transform) continue;
                Destroy(child);
            }

            foreach (var kitchenObjectSo in recipeSo.kitchenObjectSoList)
            {
                var iconTemplateGameObject = Instantiate(iconTemplate, iconContainer.transform);
                iconTemplateGameObject.gameObject.SetActive(true);
                iconTemplateGameObject.GetComponent<Image>().sprite = kitchenObjectSo.sprite;
            }
        }
    }
}
