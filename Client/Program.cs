
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Common;



//  방개념
//  유저객체 정보를 넘기는 것이랑, 

Deserialize deserialize = new();

while (true)
{
    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 123);
    Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    socket.Connect(endPoint);

    for (int i = 0; i < 5; ++i)
    {
        byte[] sendBuff = Encoding.UTF8.GetBytes("hi com2us");
        int sendBytes = socket.Send(sendBuff);   
    }
    // 
    byte[] recvBuff = new byte[1024];
    int recvLen = socket.Receive(recvBuff);
    
    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvLen);
    Console.WriteLine($"[From Server] {recvData}");
    
    User user = deserialize.UserDesirialx(recvData);
    Console.WriteLine($"[FROM Server] : Your Id : {user.UserId}, Your Value : {user.Value}");

    /*
     *  역직렬화해서 파싱해서 -> 방 찾아서 . 입장할 방번호 다시 서버로.
     * 
     */
    Thread.Sleep(2000);
}

void OnConnected(EndPoint endPoint)
{
    Packet packet = new Packet
    {
        size = 2,
        packetId = (ushort)PacketID.PlayerInfoReq,
    };
    
    //보낸다 
    for (int i = 0; i < 5; i++)
    {
        ArraySegment<byte> arraySegment = new ArraySegment<byte>();
        
        bool success = true;

        success &= BitConverter.TryWriteBytes(
            new Span<byte>(arraySegment.Array, arraySegment.Offset, arraySegment.Count), packet.size);
        
        
        //메모리 할당을 해야한다 
        //BITCONVERTER를 계속 사용하면 내부적으로 NEW bYTE를 많이해야서 성능저하.
        
        byte[] size = BitConverter.GetBytes(packet.size);
        byte[] packetId = BitConverter.GetBytes(packet.packetId);
        byte[] playerId = BitConverter.GetBytes(packet.packetId); //수정!

        ushort count = 0;
        Array.Copy(size, 0, arraySegment.Array, arraySegment.Offset + 0, 2);
        count += 2;
            //...
            
        //버퍼의 내용을 합치고 send..
        //Send(sendBuff)..

    }
}
