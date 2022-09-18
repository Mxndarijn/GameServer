using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using ClientSide.VR;
using GameServer;
using Newtonsoft.Json.Linq;

namespace SharedGameLogic.GameData;

public class Game
{
    private Thread GameThread { get; set; }
    private List<User> _users = new();
    private string name;
    public Game(List<ClientData> clients)
    {
        name = "Game-" + new Random().Next(100, 999);
        foreach (var client in clients)
        {
            _users.Add(new User(client));
        }
        GameThread = new Thread(Run);
        GameThread.Start();
        
        SendMessageToAllUsers(JsonFileReader.GetObjectAsString("Server\\GameCreated", new Dictionary<string, string>()
        {
            {"_name_", name}
        }));
        
        
    }

    public void Run()
    {
        
    }

    public void SendMessageToAllUsers(string s)
    {
        _users.ForEach(u =>
        {
            DataCommunication.SendData(u.ClientData.stream, s);
        });
    }

}