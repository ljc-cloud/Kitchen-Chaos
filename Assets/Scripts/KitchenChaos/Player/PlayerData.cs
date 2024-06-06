using System;
using Unity.Collections;
using Unity.Netcode;

namespace KitchenChaos.Player
{
    /// <summary>
    /// 实现IEquatable 和 INetworkSerializable 使其能作为NetworkVariable
    /// </summary>
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
    {
        public ulong ClientId;
        public int ColorId;
        public FixedString64Bytes PlayerName;
        public FixedString64Bytes PlayerId;

        public bool Equals(PlayerData other)
        {
            return ClientId == other.ClientId &&
                ColorId == other.ColorId &&
                PlayerName == other.PlayerName &&
                PlayerId == other.PlayerId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref ColorId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref PlayerId);
        }
    }
}
