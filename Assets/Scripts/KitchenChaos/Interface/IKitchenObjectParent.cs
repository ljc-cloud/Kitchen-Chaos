using KitchenChaos.SO;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Interface
{
    /// <summary>
    /// KitchenObject的父物品
    /// </summary>
    public interface IKitchenObjectParent
    {
        /// <summary>
        /// 属于这个KitchenObjectParent的厨房物品KitchenObject
        /// </summary>
        KitchenObject KitchenObj { get; set; }
        /// <summary>
        /// KitchenObjectParent 上厨房物品KitchenObject 的放置位置
        /// </summary>
        Transform KitchenObjectFollowTransform { get; }
        /// <summary>
        /// 是否已持有KitchenObject
        /// </summary>
        bool HasKitchenObject { get; }
        /// <summary>
        /// 清空KitchenObject
        /// </summary>
        void ClearKitchenObject();
        /// <summary>
        /// 获取KitchenObjectParent的网络组件 NetworkObject
        /// </summary>
        /// <returns></returns>
        NetworkObject GetNetworkObject();
    }
}