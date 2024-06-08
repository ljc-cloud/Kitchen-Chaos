using KitchenChaos.Interface;
using KitchenChaos.Player;
using System;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Counter
{
    /// <summary>
    /// BaseCounter ����Counter�Ļ���ʵ��
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
        /// E�����������á�ȡ��
        /// </summary>
        /// <param name="player"></param>
        public virtual void Interact(PlayerControl player)
        {
            Debug.LogError("BaseCounter InteractAltinate Invoked!");
        }
        /// <summary>
        /// F������
        /// </summary>
        /// <param name="player"></param>
        public virtual void InteractAlternate(PlayerControl player)
        {
            Debug.LogError("BaseCounter InteractAlternate Invoked!");
        }

        /// <summary>
        /// ����̬��Դ
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