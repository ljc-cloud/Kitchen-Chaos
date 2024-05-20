using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.SO
{
    [CreateAssetMenu]
    public class KitchenObjectSO : ScriptableObject
    {
        public GameObject prefab;
        public Sprite sprite;
        public string objectName;
    }
}
