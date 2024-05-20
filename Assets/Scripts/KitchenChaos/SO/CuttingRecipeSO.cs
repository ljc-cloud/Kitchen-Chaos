using UnityEngine;

namespace KitchenChaos.SO
{
    [CreateAssetMenu]
    public class CuttingRecipeSO : ScriptableObject
    {
        public KitchenObjectSO input;
        public KitchenObjectSO output;
        public int cuttingProgressMax;
    }
}