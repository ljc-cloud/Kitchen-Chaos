using KitchenChaos.Counter;
using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Manager
{
    /// <summary>
    /// 清理静态资源类
    /// </summary>
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
