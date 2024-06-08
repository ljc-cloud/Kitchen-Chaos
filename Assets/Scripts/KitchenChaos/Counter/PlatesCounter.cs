using System;
using KitchenChaos.Manager;
using KitchenChaos.Player;
using KitchenChaos.SO;
using Unity.Netcode;
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
            // 规定只有Server能生成盘子，然后广播到所有Client上进行同步
            if (!IsServer) return;
            _plateSpawnTimer += Time.deltaTime;
            if (_plateSpawnTimer > plateSpawnTime)
            {
                if (_plateSpawnAmount < plateSpawnAmountMax)
                {
                    SpawnPlateServerRpc();
                }
            }
        }

        [ServerRpc]
        private void SpawnPlateServerRpc()
        {
            SpawnPlateClientRpc();
        }
        /// <summary>
        /// 广播所有Clients 进行盘子生成同步
        /// </summary>
        [ClientRpc]
        private void SpawnPlateClientRpc()
        {
            OnPlateSpawn?.Invoke(this, EventArgs.Empty);
            _plateSpawnAmount++;
            _plateSpawnTimer = 0f;
        }

        public override void Interact(PlayerControl player)
        {
            if (player.HasKitchenObject)
            {
                return;
            }
            if (GameManager.Instance.IsGamePause)
            {
                return;
            }
            KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
            InteractLogicServerRpc();
        }

        /// <summary>
        /// 进行广播同步，同步减少一个盘子
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }

        [ClientRpc]
        private void InteractLogicClientRpc()
        {
            _plateSpawnAmount--;
            OnPlateRemove?.Invoke(this, EventArgs.Empty);
        }
    }
}