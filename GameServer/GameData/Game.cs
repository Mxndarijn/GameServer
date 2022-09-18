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
            
            Thread.Sleep(30000);
            
            SendMessageToAllUsers(JsonFileReader.GetObjectAsString("Server\\AnswerResponse", new Dictionary<string, string>()
            {
                {"_answer_", "no-time"},
                {"_questionId_", currentQuestion.id.ToString()}
                            
            }));
        }
    }

    public void SendMessageToAllUsers(string s)
    {
        _users.ForEach(u =>
        {
            DataCommunication.SendData(u.ClientData.Stream, s);
            u.ClientData.Stream.Flush();
        });
    }

    public void SendMessageToUser(User u, string s)
    {
        DataCommunication.SendData(u.ClientData.Stream, s);
        u.ClientData.Stream.Flush();
    }

    public void HandleGameMessage(ClientData data, JObject json)
    {
        switch (json["id2"].ToObject<string>())
        {
            case "answer":
            {
                try
                {
                    if (currentQuestion.CheckAnswer(GetUserByClientData(data), json["data"]["answer"].ToObject<int>()))
                    {
                        SendMessageToUser(GetUserByClientData(data), JsonFileReader.GetObjectAsString("Server\\AnswerResponse", new Dictionary<string, string>()
                        {
                            {"_answer_", "Correct"},
                            {"_questionId_", json["data"]["question-id"].ToObject<string>()}
                            
                        }));
                    }
                    else
                    {
                        SendMessageToUser(GetUserByClientData(data), JsonFileReader.GetObjectAsString("Server\\AnswerResponse", new Dictionary<string, string>()
                        {
                            {"_answer_", "Wrong"},
                            {"_questionId_", json["data"]["question-id"].ToObject<string>()}
                        }));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                break;
            }
        }
    }

    public User GetUserByClientData(ClientData data)
    {
        {
            try
            {
                User user = _users.First(u => u != null && u.ClientData == data)!;
                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}