using System.Net.Sockets;

namespace SharedGameLogic.GameData;

public class User
{
    public TcpClient Client { get; private set; }
    public NetworkStream Stream { get; private set; }
    public int Score { get; set; }
    public string UserName { get; set; }
    
    public User(TcpClient client)
    {
        this.Client = client;
        this.Score = 0;
        this.UserName = "Onbekend";
        this.Stream = Client.GetStream();
    }
}