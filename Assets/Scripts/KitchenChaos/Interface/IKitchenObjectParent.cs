using KitchenChaos.SO;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Interface
{
    /// <summary>
    /// KitchenObject�ĸ���Ʒ
    /// </summary>
    public interface IKitchenObjectParent
    {
        /// <summary>
        /// �������KitchenObjectParent�ĳ�����ƷKitchenObject
        /// </summary>
        KitchenObject KitchenObj { get; set; }
        /// <summary>
        /// KitchenObjectParent �ϳ�����ƷKitchenObject �ķ���λ��
        /// </summary>
        Transform KitchenObjectFollowTransform { get; }
        /// <summary>
        /// �Ƿ��ѳ���KitchenObject
        /// </summary>
        bool HasKitchenObject { get; }
        /// <summary>
        /// ���KitchenObject
        /// </summary>
        void ClearKitchenObject();
        /// <summary>
        /// ��ȡKitchenObjectParent��������� NetworkObject
        /// </summary>
        /// <returns></returns>
        NetworkObject GetNetworkObject();
    }
}