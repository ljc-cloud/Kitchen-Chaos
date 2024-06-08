using KitchenChaos.Interface;
using KitchenChaos.Player;
using System;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Counter
{
    /// <summary>
    /// BaseCounter 所有Counter的基类实现
    /// </summary>
    public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
    {
        [SerializeField] private Transform counterTopPoint;

        public static event EventHandler OnSomethingPlaced;

        protected KitchenObject _kitchenObj;

        public KitchenObject KitchenObj
        {
            get
            {
                return _kitchenObj;
            }
            set
            {
                _kitchenObj = value;
                if (_kitchenObj != null)
                    OnSomethingPlaced?.Invoke(this, EventArgs.Empty);
            }
        }

        public Transform KitchenObjectFollowTransform => counterTopPoint;

        public bool HasKitchenObject => KitchenObj != null;
        public void ClearKitchenObject()
        {
            _kitchenObj = null;
        }

        /// <summary>
        /// E键交互，放置、取走
        /// </summary>
        /// <param name="player"></param>
        public virtual void Interact(PlayerControl player)
        {
            Debug.LogError("BaseCounter InteractAltinate Invoked!");
        }
        /// <summary>
        /// F键交互
        /// </summary>
        /// <param name="player"></param>
        public virtual void InteractAlternate(PlayerControl player)
        {
            Debug.LogError("BaseCounter InteractAlternate Invoked!");
        }

        /// <summary>
        /// 清理静态资源
        /// </summary>
        public static void ResetStaticData()
        {
            OnSomethingPlaced = null;
        }

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }
    }
}