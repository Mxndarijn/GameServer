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
    private readonly string name;
    private List<Question> _questions = new();
    private Question currentQuestion;
    public Game(List<ClientData> clients)
    {
        name = "Game-" + new Random().Next(100, 999);
        foreach (var client in clients)
        {
            _users.Add(new User(client));
        }
        SendMessageToAllUsers(JsonFileReader.GetObjectAsString("Server\\GameCreated", new Dictionary<string, string>()
        {
            {"_name_", name}
        }));
        Thread.Sleep(10);
        GameThread = new Thread(Run);
        GameThread.Start();
        
        
        
    }

    public void Run()
    {
        while (true)
        {
            currentQuestion = new Question();
            _questions.Add(currentQuestion);
            
            SendMessageToAllUsers(currentQuestion.GetMessageToJson());
            
            Thread.Sleep(100000);
        }
    }

    public void SendMessageToAllUsers(string s)
    {
        _users.ForEach(u =>
        {
            DataCommunication.SendData(u.ClientData.Stream, s);
        });
    }

}