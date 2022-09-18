using System.Net.Sockets;
using System.Text;

class Client
{
    private TcpClient _client;

    public Client()
    {
        try
        {
            _client = new("86.95.122.200", 2460);
            WriteMessageToServer("RequestConnection");
        }
        catch
        {
            Console.WriteLine("Could not connect with server...");
        }

    }

    public void WriteMessageToServer(string message)
    {
        var stream = new StreamWriter(_client.GetStream(), Encoding.ASCII);
        {
            stream.WriteLine(message);
            stream.Flush();
        }
    }

    public string ReadResponseMessage()
    {
        var stream = new StreamReader(_client.GetStream(), Encoding.ASCII);
        {
            return stream.ReadLine();
        }
    }
}