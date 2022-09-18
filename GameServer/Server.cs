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
    private List<ClientData?> _users = new();
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
                client.Game = game;
            }
            _waiters.Clear();
            Console.WriteLine("Game Created");
        }
    }
    
    public void HandleMessage(TcpClient client, JObject json)
    {
        ClientData? data = GetClientDataByTcpClient(client);
        if (data == null)
        {
            Console.WriteLine($"Received message from unkown source: \n {json}");
            return;
        }
        switch (json["id"].ToObject<string>())
        {
            case "username":
            {
                data.UserName = json["data"]["name"].ToObject<string>();
                Console.WriteLine("Username updated...");
                if (data.Game == null && !_waiters.Contains(data))
                {
                    Console.WriteLine($"Added {data.UserName} to the waiting list for a game");
                    _waiters.Add(data);
                }
                CheckGameStart();
                break;
            }
        }
        
        string username = data.UserName != null ? data.UserName : "(GeenUserName)";
        Console.WriteLine($"Received message from {username}");
        Console.WriteLine(json);
    }

    private ClientData? GetClientDataByTcpClient(TcpClient client)
    {
        try
        {
            ClientData data = _users.First(u => u != null && u.Client == client)!;
            return data;
        }
        catch
        {
            return null;
        }
    }
}