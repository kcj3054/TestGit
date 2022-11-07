using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Common;
using Microsoft.CSharp.RuntimeBinder;

namespace Server;

//미사용 
public abstract class PacketSession
{
    public readonly int HeaderSize = 2;
    
    //[size(2)][packetId(2)][..][size(2)[packetId(2)][..]..
    public int OnRecv(ArraySegment<byte> buffer)
    {
        int processLen = 0;

        while (true)
        {
            //packet의 header도 오지않았다 
            if (buffer.Count < HeaderSize)
            {
                break;
            }
            
            //패킷이 완전체로 도착했는지 확인 
            ushort dataSize  = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            
            if (buffer.Count < dataSize)
            {
                break;
            }
            
            //여기까지 왔으면 패킷 조립 가능 
            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

            processLen += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            
        }

        return processLen;
    }

    public void OnRecvPacket(ArraySegment<byte> buffer)
    {
        //구버전..
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
        
        Console.WriteLine($"RecvPacket Id : {id} and RecvPacketSize : {size}");
        
        ushort count = 0;
        //신버전 아직 미완성..
        ushort size2 = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id2 = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

    }
}

public class Session
{
    private Socket? _socket;
    private Int32 _disconnected = 0;
    private Deserialize _deserialize = new();
    private ushort _userCount = 0;  // 홀수번째마다 방을 생성해야한다. userCount에 접근할 때는 당연히 lock을 걸어줘야한다 
    private ushort _roomNumber = 0; // 위험 
    private RoomManager _roomManager = new();
    private object _lock = new();
    
    public void Init(Socket socket)
    {
        _socket = socket;
        SocketAsyncEventArgs recvArgs = new();
        SocketAsyncEventArgs sendArgs = new();
        
        recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
    //    sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
        
        // recvArgs.UserToken = 1; session의 정보를 넣어 둘 수 있음 
        recvArgs.SetBuffer(new byte[1024], 0, 1024);
        
        RegisterRecv(recvArgs);
    }

    public void RegisterRecv(SocketAsyncEventArgs args)
    {
        bool pending =  _socket.ReceiveAsync(args);
       if (pending == false)
       {
           OnRecvCompleted(null, args);
       }
    }
    
    public void OnRecvCompleted(Object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {

            lock (_lock)
            {
                _userCount++;
                //비정상 
            }

            string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
            Console.WriteLine($"[From Client] : {recvData}");
            Console.WriteLine($"현재 유저는 {_userCount}입니다");
            
            User recvUser = _deserialize.UserDesirialx(recvData);
            //User recvUser = JsonSerializer.Deserialize<User>(recvData);

            Console.WriteLine($"{recvUser.Value} : ==============================================================");
            
            if (_userCount % 2 == 1)
            {
                lock (_lock)
                {
                    _roomNumber++;
                    Console.WriteLine($"현재 룸 {_roomNumber}번이 생성되었습니다. ");
                }

                //새로운 방을 생성 
                Room room = new Room
                {
                    RoomNumber = _roomNumber,
                    UserList = null,
                };

                lock (_lock)
                {
                    room?.UserList?.Add(recvUser);
                    _roomManager?.RoomList?.Add(room);
                }
               
            }
            else
            {
                //새로운 방을 생성하지 않아도 될때는 이미 존재하는 방의 번호에 들어가자 
                // _roomManager.RoomList[_roomNumber]
            }

            // Send를 호출하기전에 보낼 정보 직렬화 
            //Ex :  더미 데이터 테스트 용 
            User user = new User()
            {
                UserId = 1,
                Value = 2
            };

            byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes(user);
            // JsonSerializer.Deserialize<User>(jsonString))
            Send(jsonString);
            RegisterRecv(args);

        }
        else
        {
            Console.WriteLine($"[Server] : OnRecvCompleted Error");
        }
    }

    public void Send(byte[] sendBuff)
    {
        _socket.Send(sendBuff);
    }
    
    void RegisterSend()
    {
        
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;
        
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
}