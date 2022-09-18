using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;
using SharedGameLogic.GameData;

namespace GameServer;

public class Server
{
    private TcpListener _listener;

    private List<TcpClient> _waiters = new();
    private Dictionary<TcpClient, Game> _userList = new();
    private Dictionary<TcpClient, string> _userNames = new();

    private Dictionary<TcpClient, ClientHandler> handler = new();
    private List<TcpClient> _connections = new();
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
            this._connections.Add(client);
            this.handler.Add(client, new ClientHandler(this,client));
            
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
                _userList.Add(client, game);
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
        Console.WriteLine("Received message");
        Console.WriteLine(json);
    }
}