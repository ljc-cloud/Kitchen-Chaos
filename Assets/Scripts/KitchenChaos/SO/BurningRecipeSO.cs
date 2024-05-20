using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.SO
{
    [CreateAssetMenu]
    public class BurningRecipeSO : ScriptableObject
    {
        public KitchenObjectSO input;
        public KitchenObjectSO output;
        public int burningTimerMax;
    }
}
