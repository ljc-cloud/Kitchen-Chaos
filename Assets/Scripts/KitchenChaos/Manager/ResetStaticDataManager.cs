using KitchenChaos.Counter;
using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Manager
{
    public class ResetStaticDataManager : MonoBehaviour
    {
        private void Awake()
        {
            BaseCounter.ResetStaticData();
            CuttingCounter.ResetStaticData();
            TrashCounter.ResetStaticData();
            PlayerControl.ResetStaticData();
        }
    }
}
