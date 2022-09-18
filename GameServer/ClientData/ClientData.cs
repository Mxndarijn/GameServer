using System.Net.Sockets;
using SharedGameLogic.GameData;

namespace GameServer;

public class ClientData
{
    public ClientHandler handler { get; set; }
    public TcpClient client { get; set; }
    public NetworkStream stream { get; set; }
    public Server server;

    public Game game { get; set; }
    
    public ClientData(Server server, TcpClient client)
    {
        this.server = server;
        this.client = client;
        this.stream = client.GetStream();
        this.handler = new ClientHandler(server, client);
    }
    
}