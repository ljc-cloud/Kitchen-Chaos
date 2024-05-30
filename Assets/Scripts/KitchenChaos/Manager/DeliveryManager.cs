using KitchenChaos.SO;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Manager
{
    public class DeliveryManager : NetworkBehaviour
    {
        [SerializeField] private RecipeListSO recipeListSo;

        public static DeliveryManager Instance { get; private set; }

        public List<RecipeSO> WaitingRecipeSoList { get; private set; }

        public event EventHandler OnRecipeSpawned;
        public event EventHandler OnRecipeCompleted;

        public event EventHandler OnRecipeSuccess;
        public event EventHandler OnRecipeFail;

        private float _spawnTimer;
        private float _spawnTimerMax = 4f;
        private int _spawnCountMax = 4;

        public int DeliverySuccessAmount { get; private set; }

        private void Awake()
        {
            Instance = this;
            WaitingRecipeSoList = new List<RecipeSO>();
            DeliverySuccessAmount = 0;
        }

        private void Update()
        {
            if (!IsServer) return;
            if (!GameManager.Instance.IsGamePlaying) return;
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                _spawnTimer = _spawnTimerMax;
                // per 4 seconds spawn a recipe
                if (WaitingRecipeSoList.Count < _spawnCountMax)
                {
                    int recipeSoIndex = UnityEngine.Random.Range(0, recipeListSo.recipeSoList.Count);
                    // Spawn a Recipe 2 WaitingRecipeSoList
                    
                    SpawnNewWaitingRecipeClientRpc(recipeSoIndex);
                }
            }
        }

        [ClientRpc]
        private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSoIndex)
        {
            RecipeSO waitingRecipeSo = recipeListSo.recipeSoList[waitingRecipeSoIndex];
            WaitingRecipeSoList.Add(waitingRecipeSo);
            OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
        }

        public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
        {
            for (int i = 0; i < WaitingRecipeSoList.Count; i++)
            {
                RecipeSO recipeSo = WaitingRecipeSoList[i];
                var plateContentMatchesRecipe = true;
                foreach (var recipekitchenObject in recipeSo.kitchenObjectSoList)
                {
                    bool ingrediantFound = false;
                    foreach (var kitchenObject in plateKitchenObject.KitchenObjectSoList)
                    {
                        if (recipekitchenObject == kitchenObject)
                        {
                            ingrediantFound = true;
                            break;
                        }
                    }

                    if (!ingrediantFound)
                    {
                        plateContentMatchesRecipe = false;
                    }
                }
                if (plateContentMatchesRecipe)
                {
                    // Player did delivery correct recipe
                    //DeliverySuccessAmount++;
                    //WaitingRecipeSoList.RemoveAt(i);
                    //OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    //OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    DeliveryCorrectRecipeServerRpc(i);
                    return;
                }
            }
            DeliveryIncorrectRecipeServerRpc();
        }


        [ServerRpc(RequireOwnership = false)]
        private void DeliveryCorrectRecipeServerRpc(int waitingRecipeSoListIndex)
        {
            DeliveryCorrectRecipeClientRpc(waitingRecipeSoListIndex);
        }
        [ClientRpc]
        private void DeliveryCorrectRecipeClientRpc(int waitingRecipeSoListIndex)
        {
            Debug.Log("Delivery CorrectRecipe");
            // Player did delivery correct recipe
            DeliverySuccessAmount++;
            WaitingRecipeSoList.RemoveAt(waitingRecipeSoListIndex);
            OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
            OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DeliveryIncorrectRecipeServerRpc()
        {
            DeliveryIncorrectRecipeClientRpc();
        }
        [ClientRpc]
        private void DeliveryIncorrectRecipeClientRpc()
        {
            Debug.Log("Delivery InCorrectRecipe");
            OnRecipeFail?.Invoke(this, EventArgs.Empty);
        }
    }

}