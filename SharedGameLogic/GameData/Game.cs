using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using ClientSide.VR;
using Newtonsoft.Json.Linq;

namespace SharedGameLogic.GameData;

public class Game
{
    private Thread GameThread { get; set; }
    private List<User> _users;
    public Game(List<TcpClient> clients)
    {
        foreach (var client in clients)
        {
            _users.Add(new User(client));
        }
        GameThread = new Thread(Run);
        GameThread.Start();
    }

    public void Run()
    {
        
    }

    public void SendMessageToAllUsers(string s)
    {
        _users.ForEach(u =>
        {
            DataCommunication.WriteTextMessage(u.Client, s);
        });
    }

}