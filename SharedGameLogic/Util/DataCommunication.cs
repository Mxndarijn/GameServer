using System.Net.Sockets;
using System.Text;

namespace ClientSide.VR;

public class DataCommunication
{

    public static void SendData(NetworkStream stream, String s)
    {
       // Console.WriteLine($"Sending data: {s}");
        Byte[] data = BitConverter.GetBytes(s.Length);
        Byte[] comman = System.Text.Encoding.ASCII.GetBytes(s);
        stream.Write(data, 0, data.Length);
        stream.Write(comman, 0, comman.Length);
    }

    public static string ReadTextMessage(TcpClient client)
    {
        var stream = new StreamReader(client.GetStream(), Encoding.ASCII);
        {
            return stream.ReadLine();
        }
    }
}