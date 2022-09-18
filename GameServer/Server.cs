using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;
using SharedGameLogic.GameData;

namespace GameServer;

public class Server
{
    private TcpListener _listener;

    private List<ClientData> _waiters = new();
    private List<ClientData> _users = new();
    private List<Game> _games = new();
    
    public delegate void MessageReceived(TcpClient client, JObject json);

    public MessageReceived OnMessage { get; }

    public Server()
    {
        OnMessage = HandleMessage;
        _listener = new TcpListener(IPAddress.Any, 2460);
        _listener.Start();
        while (true)
        {
            Console.WriteLine("Waiting for connection");
            TcpClient client = _listener.AcceptTcpClient();
            Console.WriteLine("Accepted client");
            this._users.Add(new ClientData(this, client));

            CheckGameStart();
            
           // new Thread(HandleIncommingRequests).Start(client);
        }
    }

    public void CheckGameStart()
    {
        if (_waiters.Count >= 2)
        {
            Game game = new Game(_waiters);
            _games.Add(game);
            foreach (var client in _waiters)
            {
                client.game = game;
            }
            _waiters.Clear();
            Console.WriteLine("Game Created");
        }
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


    public void HandleMessage(TcpClient client, JObject json)
    {
        switch (json["id"].ToObject<string>())
        {
            case "username":
            {
                
                break;
            }
        }
        Console.WriteLine("Received message");
        Console.WriteLine(json);
    }
}