// See https://aka.ms/new-console-template for more information


using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

BinaryFormatter binaryFormatter = new();
Int32 value = 0;
while (true)
{
    TcpClient client = new();
    client.Connect(IPAddress.Parse("127.0.0.1"), 123);
    NetworkStream networkStream = client.GetStream();
  
    Console.WriteLine("1.가위, 2.바위, 3.보 입력 : ");
    value = Int32.Parse(Console.ReadLine());
    
    binaryFormatter.Serialize(networkStream, value);

    value = (int)binaryFormatter.Deserialize(networkStream);

    //결과를 출력 
    Console.WriteLine(value);
}