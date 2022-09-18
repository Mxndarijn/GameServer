using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer;

public class Server
{
    private TcpListener _listener;

    public Server()
    {
        IPAddress localhost = IPAddress.Parse("127.0.0.1");
        _listener = new TcpListener(localhost, 2460);
        _listener.Start();
        
        while (true)
        {
            Console.WriteLine("Waiting for connection");
        
            TcpClient client = _listener.AcceptTcpClient();

            Console.WriteLine("Accepted client");
            
            new Thread(HandleIncommingRequests).Start(client);
        }
    }

    public void HandleIncommingRequests(object obj)
    {
        TcpClient client = obj as TcpClient;

        bool done = false;
        while (!done)
        {
            string received = ReadTextMessage(client);
            Console.WriteLine("Received: {0}", received);

            done = received.Equals("bye");
            if (done) WriteTextMessage(client, "BYE");
            else WriteTextMessage(client, "OK");
        }
        client.Close();
        Console.WriteLine("Connection closed");
    }
    
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