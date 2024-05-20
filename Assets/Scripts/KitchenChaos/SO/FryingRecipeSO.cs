using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.SO
{
    [CreateAssetMenu]
    public class FryingRecipeSO : ScriptableObject
    {
        public KitchenObjectSO input;
        public KitchenObjectSO output;
        public int fryingTimerMax;
    }
}
