namespace Common;

public class Packet
{
    public ushort size;
    public ushort packetId;
}

public class PlayerInfoReq : Packet
{
    public long playerId;
}

public class PlayerInfoRes : Packet
{
  
}

public enum PacketID
{
    PlayerInfoReq = 1,
    PlayerInfoRes = 2,
    
}