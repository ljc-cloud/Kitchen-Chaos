using System;
using System.Linq;
using KitchenChaos.Interface;
using KitchenChaos.Player;
using KitchenChaos.SO;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class StoveCounter : BaseCounter, IHasProgress
    {
        [SerializeField] private FryingRecipeSO[] fryingRecipeSoArr;
        [SerializeField] private BurningRecipeSO[] burningRecipeSoArr;

        private float _fryingTimer;
        private float _burningTimer;

        private FryingRecipeSO _fryingRecipeSo;
        private BurningRecipeSO _burningRecipeSo;

        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
        public event EventHandler<IHasProgress.OnProgressbarChangedEventArgs> OnProgressbarChanged;

        public bool IsFried => _state == State.Fried;

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

        private State _state;

        private void Update()
        {
            if (!HasKitchenObject)
            {
                return;
            }

            switch (_state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    _fryingTimer += Time.deltaTime;
                    OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                    {
                        progressNormalized = _fryingTimer / _fryingRecipeSo.fryingTimerMax
                    });
                    if (_fryingTimer >= _fryingRecipeSo.fryingTimerMax)
                    {
                        KitchenObj.DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
                        _burningTimer = 0f;
                        _fryingTimer = 0f;
                        _burningRecipeSo = GetBurningRecipeSoByInput(_fryingRecipeSo.output);
                        _state = State.Fried;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = _state });
                    }

                    break;
                case State.Fried:
                    _burningTimer += Time.deltaTime;
                    OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                    {
                        progressNormalized = _burningTimer / _burningRecipeSo.burningTimerMax
                    });
                    if (_burningTimer >= _burningRecipeSo.burningTimerMax)
                    {
                        KitchenObj.DestroySelf();
                        KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);
                        _burningTimer = 0f;
                        _state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = _state });
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
                    if (TryGetFryingRecipeWithInput(out FryingRecipeSO fryingRecipeSo,
                            player.KitchenObj.KitchenObjectSo))
                    {
                        _fryingRecipeSo = fryingRecipeSo;
                        player.KitchenObj.SetKitchenObjectParent(this);
                        _state = State.Frying;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = _state });
                        OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                        {
                            progressNormalized = _fryingTimer / _fryingRecipeSo.fryingTimerMax
                        });
                    }
                    else if (TryGetBurningRecipeWithInput(out BurningRecipeSO burningRecipeSo,
                                 player.KitchenObj.KitchenObjectSo))
                    {
                        _burningRecipeSo = burningRecipeSo;
                        player.KitchenObj.SetKitchenObjectParent(this);
                        _state = State.Fried;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = _state });
                        OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                        {
                            progressNormalized = _burningTimer / _burningRecipeSo.burningTimerMax
                        });
                    }
                }
            }
            else
            {
                if (!player.HasKitchenObject)
                {
                    KitchenObj.SetKitchenObjectParent(player);
                    _state = State.Idle;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = _state });
                    OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                }
                else
                {
                    if (player.KitchenObj.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                    {
                        plateKitchenObject.AddIngredient(KitchenObj.KitchenObjectSo);
                        KitchenObj.DestroySelf();
                        _state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = _state });
                        OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
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