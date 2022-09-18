using System.Net;
using System.Net.Sockets;

namespace GameServer;

public class Server
{
    private TcpListener _listener;
    public Server()
    {
        IPAddress localhost = IPAddress.Parse("127.0.0.1");
        _listener = new TcpListener(localhost, 2460);
        
        new Thread(HandleIncommingRequests).Start();
    }

    public void HandleIncommingRequests()
    {
        Console.WriteLine("Waiting for connection");
        
        TcpClient client = _listener.AcceptTcpClient();
        
    }

}