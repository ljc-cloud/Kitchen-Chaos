using System;
using System.Linq;
using KitchenChaos.Interface;
using KitchenChaos.Network;
using KitchenChaos.Player;
using KitchenChaos.SO;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class StoveCounter : BaseCounter, IHasProgress
    {
        [SerializeField] private FryingRecipeSO[] fryingRecipeSoArr;
        [SerializeField] private BurningRecipeSO[] burningRecipeSoArr;

        // Only Server Can Write These Variavle
        private NetworkVariable<float> _fryingTimer = new NetworkVariable<float>(0f);
        private NetworkVariable<float> _burningTimer = new NetworkVariable<float>(0f);

        private FryingRecipeSO _fryingRecipeSo;
        private BurningRecipeSO _burningRecipeSo;

        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
        public event EventHandler<IHasProgress.OnProgressbarChangedEventArgs> OnProgressbarChanged;

        public bool IsFried => _state.Value == State.Fried;

        public class OnStateChangedEventArgs : EventArgs
        {
            public State state;
        }

        public enum State
        {
            Idle,
            Frying,
            Fried,
            Burned
        }

        private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);

        public override void OnNetworkSpawn()
        {
            _fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
            _burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
            _state.OnValueChanged += State_OnValueChanged;
        }

        private void State_OnValueChanged(State previousState, State newState)
        {
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = _state.Value });
            if (_state.Value == State.Fried)
            {
                _burningRecipeSo = GetBurningRecipeSoByInput(_fryingRecipeSo.output);
            }
            if (_state.Value == State.Idle || _state.Value == State.Burned)
            {
                OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }

        private void BurningTimer_OnValueChanged(float previousValue, float newValue)
        {
            var burningTimerMax = _burningRecipeSo != null ? _burningRecipeSo.burningTimerMax : 1f;
            OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
            {
                progressNormalized = _burningTimer.Value / burningTimerMax
            });
        }

        private void FryingTimer_OnValueChanged(float previousValue, float newValue)
        {
            var fryingTimerMax = _fryingRecipeSo != null ? _fryingRecipeSo.fryingTimerMax : 1f;
            OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
            {
                progressNormalized = _fryingTimer.Value / fryingTimerMax
            });
        }

        private void Update()
        {
            if (!HasKitchenObject)
            {
                return;
            }
            if (!IsServer)
            {
                return;
            }

            switch (_state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    _fryingTimer.Value += Time.deltaTime;
                    if (_fryingTimer.Value >= _fryingRecipeSo.fryingTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(KitchenObj);
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
                        _fryingTimer.Value = 0f;
                        _burningTimer.Value = 0f;
                        _state.Value = State.Fried;
                        SetStoveCounterRecipeClientRpc(
                            KitchenGameMultiPlayer.Instance.GetKitchenObjectIndex(this.KitchenObj.KitchenObjectSo),
                            true
                            );
                    }
                    break;
                case State.Fried:
                    _burningTimer.Value += Time.deltaTime;
                    if (_burningTimer.Value >= _burningRecipeSo.burningTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(KitchenObj);
                        KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);
                        _burningTimer.Value = 0f;
                        _state.Value = State.Burned;
                    }
                    break;
                case State.Burned:
                    break;
            }
        }

        public override void Interact(PlayerControl player)
        {
            if (!HasKitchenObject)
            {
                if (!player.HasKitchenObject)
                {
                }
                else
                {
                    if (TryGetFryingRecipeWithInput(out FryingRecipeSO fryingRecipeSo, player.KitchenObj.KitchenObjectSo))
                    {
                        var index = KitchenGameMultiPlayer.Instance.GetKitchenObjectIndex(player.KitchenObj.KitchenObjectSo);
                        player.KitchenObj.SetKitchenObjectParent(this);
                        InteractLogicPlaceObjectOnCounterServerRpc(index, false);
                    }
                    else if (TryGetBurningRecipeWithInput(out BurningRecipeSO burningRecipeSo,
                                 player.KitchenObj.KitchenObjectSo))
                    {
                        var index = KitchenGameMultiPlayer.Instance.GetKitchenObjectIndex(player.KitchenObj.KitchenObjectSo);
                        player.KitchenObj.SetKitchenObjectParent(this);
                        InteractLogicPlaceObjectOnCounterServerRpc(index, true);
                    }
                }
            }
            else
            {
                if (!player.HasKitchenObject)
                {
                    KitchenObj.SetKitchenObjectParent(player);
                    SetStoveIdleServerRpc();
                }
                else
                {
                    if (player.KitchenObj.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                    {
                        plateKitchenObject.AddIngredient(KitchenObj.KitchenObjectSo);
                        KitchenObject.DestroyKitchenObject(KitchenObj);
                        SetStoveIdleServerRpc();
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetStoveIdleServerRpc()
        {
            _state.Value = State.Idle;
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceObjectOnCounterServerRpc(int index, bool fried)
        {
            _fryingTimer.Value = 0f;
            _burningTimer.Value = 0f;
            _state.Value = fried ? State.Fried : State.Frying;
            SetStoveCounterRecipeClientRpc(index, fried);
        }
        [ClientRpc]
        private void SetStoveCounterRecipeClientRpc(int index, bool fried)
        {
            var kitchenObjectSo = KitchenGameMultiPlayer.Instance.GetKitchenObjectSOByIndex(index);
            if (!fried)
            {
                _fryingRecipeSo = GetFryingRecipeSoByInput(kitchenObjectSo);
            }
            else
            {
                _burningRecipeSo = GetBurningRecipeSoByInput(kitchenObjectSo);
            }
        }

        private FryingRecipeSO GetFryingRecipeSoByInput(KitchenObjectSO inputKitchenObjectSo)
        {
            return fryingRecipeSoArr.FirstOrDefault(item => Equals(item.input, inputKitchenObjectSo));
        }

        private BurningRecipeSO GetBurningRecipeSoByInput(KitchenObjectSO inputKitchenObjectSo)
        {
            return burningRecipeSoArr.FirstOrDefault(item => Equals(item.input, inputKitchenObjectSo));
        }

        private bool TryGetFryingRecipeWithInput(out FryingRecipeSO outCuttingRecipeSo,
            KitchenObjectSO inputKitchenObjectSo)
        {
            outCuttingRecipeSo = GetFryingRecipeSoByInput(inputKitchenObjectSo);
            return outCuttingRecipeSo != null;
        }

        private bool TryGetBurningRecipeWithInput(out BurningRecipeSO outBurningRecipeSo,
            KitchenObjectSO inputKitchenObjectSo)
        {
            outBurningRecipeSo = GetBurningRecipeSoByInput(inputKitchenObjectSo);
            return outBurningRecipeSo != null;
        }

        private KitchenObjectSO GetOutputByInput(KitchenObjectSO inputKitchenObjectSo)
        {
            var fryingRecipeSo = GetFryingRecipeSoByInput(inputKitchenObjectSo);
            return fryingRecipeSo != null ? fryingRecipeSo.output : null;
        }
    }
}