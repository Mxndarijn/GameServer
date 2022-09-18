using System.Net.Sockets;
using SharedGameLogic.GameData;

namespace GameServer;

public class ClientData
{
    public ClientHandler Handler { get; set; }
    public TcpClient Client { get; set; }
    public NetworkStream Stream { get; set; }
    public Server Server { get; set; }
    public string UserName { get; set; }

    public Game? Game { get; set; }
    
    public ClientData(Server server, TcpClient client)
    {
        this.Server = server;
        this.Client = client;
        this.Stream = client.GetStream();
        this.Game = null;
        this.Handler = new ClientHandler(server, client);
    }
    
}