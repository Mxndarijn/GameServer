using System.Net.Sockets;
using GameServer;

namespace SharedGameLogic.GameData;

public class User
{
    public ClientData ClientData { get; private set; }
    public int Score { get; set; }
    public string UserName { get; set; }
    
    public User(ClientData clientData)
    {
        this.ClientData = clientData;
        this.Score = 0;
        this.UserName = "Onbekend";
    }
}