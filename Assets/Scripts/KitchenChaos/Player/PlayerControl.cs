using System;
using System.Collections.Generic;
using KitchenChaos.Counter;
using KitchenChaos.Interface;
using KitchenChaos.Manager;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Player
{
    public class PlayerControl : NetworkBehaviour, IKitchenObjectParent
    {
        public static PlayerControl LocalInstance { get; private set; }

        public static event EventHandler OnAnyPlayerSpawned;
        public static event EventHandler OnAnyPickUpSomething;
        public bool IsWalking { get; private set; }

        public event EventHandler OnPickupSomething;
        public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
        public class OnSelectedCounterChangedEventArgs : EventArgs
        {
            public BaseCounter SelectedCounter;
        }

        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float rotateSpeed = 7f;
        [SerializeField] private LayerMask counterLayer;
        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private List<Vector3> spawnPositionList;
        [SerializeField] private Transform kitchenObjectHolderPoint;

        private Vector3 _lastInteractDir;
        private BaseCounter _selectedCounter;

        private KitchenObject _kitchenObj;
        public KitchenObject KitchenObj
        {
            get
            {
                return _kitchenObj;
            }
            set
            {
                _kitchenObj = value;
                if (KitchenObj != null)
                {
                    OnPickupSomething?.Invoke(this, EventArgs.Empty);
                    OnAnyPickUpSomething?.Invoke(this, EventArgs.Empty);
                }

            }
        }
        public Transform KitchenObjectFollowTransform => kitchenObjectHolderPoint;
        public bool HasKitchenObject => _kitchenObj != null;

        /// <summary>
        /// 当物体在网络中生成时调用
        /// </summary>
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                LocalInstance = this;
            }
            transform.position = spawnPositionList[(int)OwnerClientId];
            OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId && HasKitchenObject)
            {
                KitchenObject.DestroyKitchenObject(KitchenObj);
            }
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            HandleMovement();
            //HandleMovementServerAuth();
            HandleInteract();
        }

        private void Start()
        {
            GameInput.Instance.OnInteractAction += GameInputOnOnInteractAction;
            GameInput.Instance.OnInteractAlternateAction += GameInputOnOnInteractAlternateAction;
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnPickupSomething = null;
            OnSelectedCounterChanged = null;
            GameInput.Instance.OnInteractAction -= GameInputOnOnInteractAction;
            GameInput.Instance.OnInteractAlternateAction -= GameInputOnOnInteractAlternateAction;
        }

        public static void ResetStaticData()
        {
            OnAnyPlayerSpawned = null;
            OnAnyPickUpSomething = null;
        }

        private void GameInputOnOnInteractAlternateAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsGamePlaying) return;
            if (_selectedCounter != null)
            {
                _selectedCounter.InteractAlternate(this);
            }
        }

        private void GameInputOnOnInteractAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsGamePlaying) return;
            if (_selectedCounter != null)
            {
                _selectedCounter.Interact(this);
            }
        }

        private void HandleInteract()
        {
            if (!GameManager.Instance.IsGamePlaying) return;
            var inputVector = GameInput.Instance.GetInputVectorNormalized();
            var interactDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;
            var interactDistance = 1.5f;
            if (interactDir != Vector3.zero)
            {
                _lastInteractDir = interactDir;
            }
            if (Physics.Raycast(transform.position, _lastInteractDir, out RaycastHit hitInfo, interactDistance, counterLayer))
            {
                if (hitInfo.transform.TryGetComponent(out BaseCounter baseCounter))
                {
                    SetSelectedCounter(baseCounter);
                }
                else
                {
                    SetSelectedCounter(null);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }


        #region Server Auth, Client Tranform Auth By Server
        private void HandleMovementServerAuth()
        {
            var inputVector = GameInput.Instance.GetInputVectorNormalized();
            HandleMovementServerRpc(inputVector);
        }

        [ServerRpc]
        private void HandleMovementServerRpc(Vector2 inputVector)
        {
            var moveDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;

            var moveDistance = moveSpeed * Time.deltaTime;
            var playerHeight = 2f;
            var playerRadius = .7f;

            var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                playerRadius, moveDir,
                moveDistance);

            if (!canMove)
            {
                // 查看在x轴上是否可以移动
                var moveDirX = new Vector3(inputVector.x, 0, 0).normalized;
                canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                    playerRadius, moveDirX,
                    moveDistance);
                if (canMove)
                {
                    moveDir = moveDirX;
                }
                else
                {
                    // 查看在z轴上是否可以移动
                    var moveDirZ = new Vector3(0, 0, inputVector.y).normalized;
                    canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                        playerRadius, moveDirZ,
                        moveDistance);
                    if (canMove)
                    {
                        moveDir = moveDirZ;
                    }
                }
            }

            if (canMove)
            {
                transform.position += moveDir * moveSpeed * Time.deltaTime;
            }

            IsWalking = moveDir != Vector3.zero;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }
        #endregion


        private void HandleMovement()
        {
            var inputVector = GameInput.Instance.GetInputVectorNormalized();
            var moveDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;

            var moveDistance = moveSpeed * Time.deltaTime;
            //var playerHeight = 2f;
            var playerRadius = .7f;
            var canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity,
                moveDistance, collisionLayer);

            if (!canMove)
            {
                // 查看在x轴上是否可以移动
                var moveDirX = new Vector3(inputVector.x, 0, 0).normalized;
                canMove = moveDir.x != 0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity,
                moveDistance, collisionLayer);
                if (canMove)
                {
                    moveDir = moveDirX;
                }
                else
                {
                    // 查看在z轴上是否可以移动
                    var moveDirZ = new Vector3(0, 0, inputVector.y).normalized;
                    canMove = moveDir.z != 0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity,
                moveDistance, collisionLayer);
                    if (canMove)
                    {
                        moveDir = moveDirZ;
                    }
                }
            }

            if (canMove)
            {
                transform.position += moveDir * moveSpeed * Time.deltaTime;
            }

            IsWalking = moveDir != Vector3.zero;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }

        private void SetSelectedCounter(BaseCounter selectedCounter)
        {
            _selectedCounter = selectedCounter;

            OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
            {
                SelectedCounter = _selectedCounter
            });
        }

        public void ClearKitchenObject() => _kitchenObj = null;

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }
    }
}