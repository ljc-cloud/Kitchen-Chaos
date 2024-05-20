using UnityEngine;
using KitchenChaos.Manager;

namespace KitchenChaos.UI
{
    public class DeliveryManagerUI : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private GameObject recipeTemplate;


        private void Awake()
        {
            recipeTemplate.SetActive(false);
        }

        private void Start()
        {
            DeliveryManager.Instance.OnRecipeSpawned += DeliveryManagerOnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeCompleted += DeliveryManagerOnRecipeCompleted; ;
            UpdateVisual();
        }

        private void DeliveryManagerOnRecipeCompleted(object sender, System.EventArgs e)
        {
            UpdateVisual();
        }

        private void DeliveryManagerOnRecipeSpawned(object sender, System.EventArgs e)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            // clear previous
            foreach (Transform child in container.transform)
            {
                if (child == recipeTemplate.transform) continue;
                Destroy(child.gameObject);
            }
            // get waitingRecipeSoList
            foreach (var recipeSo in DeliveryManager.Instance.WaitingRecipeSoList)
            {
                var recipeTemplateGameObject = Instantiate(recipeTemplate, container.transform);
                recipeTemplateGameObject.SetActive(true);
                recipeTemplateGameObject.GetComponent<RecipeTemplateUI>().SetRecipeSo(recipeSo);
            }
        }
    }
}
