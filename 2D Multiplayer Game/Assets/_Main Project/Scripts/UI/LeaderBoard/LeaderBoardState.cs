using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderBoardState : INetworkSerializable,IEquatable<LeaderBoardState>
{
    public ulong ClientID;
    public FixedString32Bytes PlayerName;
    public int Coins;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }

    public bool Equals(LeaderBoardState other)
    {
        return ClientID == other.ClientID && PlayerName.Equals(other.PlayerName) && Coins == other.Coins;
    }
}
