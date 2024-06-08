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
            // �涨ֻ��Server���������ӣ�Ȼ��㲥������Client�Ͻ���ͬ��
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
        /// �㲥����Clients ������������ͬ��
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
        /// ���й㲥ͬ����ͬ������һ������
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