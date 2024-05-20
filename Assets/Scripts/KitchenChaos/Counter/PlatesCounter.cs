using System;
using KitchenChaos.Player;
using KitchenChaos.SO;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class PlatesCounter : BaseCounter
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSo;
        [SerializeField] private int plateSpawnAmountMax;
        [SerializeField] private float plateSpawnTime;

        private float _plateSpawnTimer;
        private int _plateSpawnAmount;

        public event EventHandler OnPlateSpawn;
        public event EventHandler OnPlateRemove;

        private void Update()
        {
            _plateSpawnTimer += Time.deltaTime;
            if (_plateSpawnTimer > plateSpawnTime)
            {
                if (_plateSpawnAmount < plateSpawnAmountMax)
                {
                    OnPlateSpawn?.Invoke(this, EventArgs.Empty);
                    _plateSpawnAmount++;
                    _plateSpawnTimer = 0f;
                }
            }
        }

        public override void Interact(PlayerControl player)
        {
            if (player.HasKitchenObject)
            {
                return;
            }

            OnPlateRemove?.Invoke(this, EventArgs.Empty);
            _plateSpawnAmount--;
            KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
        }
    }
}