using System.Net.Sockets;
using System.Text;

namespace ClientSide.VR;

public class DataCommunication
{
    public static void WriteTextMessage(TcpClient client, string message)
    {
        var stream = new StreamWriter(client.GetStream(), Encoding.ASCII, -1, true);
        {
            stream.WriteLine(message);
            stream.Flush();
        }
    }

    public static string ReadTextMessage(TcpClient client)
    {
        var stream = new StreamReader(client.GetStream(), Encoding.ASCII);
        {
            return stream.ReadLine();
        }
    }
}