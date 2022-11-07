
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using Common;
using Server;

/*
 * 방
 */
Listener listener = new();

IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 123);

RoomManager roomManager = new();
listener.Init(endPoint, OnAcceptHandler);
// list - session의 리스트, 방

while (true)
{
    ;
}

// "size=1,packetId=2"

User user = new User
{
    UserId = 1,
    Value = 2
};
Room room = new Room
{
    RoomNumber = 1,
    UserList = null,
};


void OnAcceptHandler(Socket clientSocket)
{
    Session session = new Session();
    session.Init(clientSocket); // Init후에 자동으로 받는다 !! 

    // var roomList = new RoomManager();
    // roomList.RoomList.Add(new Room() {RoomNumber = 1, UserList = new List<User>()});
    // session을 list에 추가
    
    // client에 방의 리스트를 보여주고.
    // client고르고.. 

    //byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes(roomList);
    // session.Send(jsonString);
    
    // StringBuilder stringBuilder = new StringBuilder();
    // foreach (var room in roomList)
    // {
    //     stringBuilder.Append($"{room.id},{room._userList.Count};");
    // }
    // byte[] sendBuff = Encoding.UTF8.GetBytes(stringBuilder.ToString());
    // session.Send(sendBuff);
    
    //Thread.Sleep(1000);
    
    // session.Disconnect();
    //예외처리 추가 
    // //recv...
    // byte[] recvBuff = new byte[1024];
    // int recvLen = clientSocket.Receive(recvBuff);
    //
    // //send..
    // byte[] sendBuff = new byte[1024];
    // clientSocket.Send(sendBuff);
}

/*
 *   
 *  액터는 여러개 
 *  각 리소스()를 관리하는 액터를 하나씩 둔다.
 *  액터는 항상 동기적으로 처리한다.
 *  액터 사이에서는 매시지를 통해서  통신한다.
 *
 */ 