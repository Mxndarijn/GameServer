using System.Net;
using System.Net.Sockets;
using System.Text;
using SharedGameLogic.GameData;

namespace GameServer;

public class Server
{
    private TcpListener _listener;

    private List<TcpClient> _waiters = new();
    private Dictionary<TcpClient, Game> _userList = new();
    private List<Game> _games = new();

    public Server()
    {
        _listener = new TcpListener(IPAddress.Any, 2460);
        _listener.Start();
        while (true)
        {
            Console.WriteLine("Waiting for connection");
            TcpClient client = _listener.AcceptTcpClient();
            Console.WriteLine("Accepted client");
            this._waiters.Add(client);
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

    public void HandleIncomingRequests(object obj)
    {
        TcpClient client = obj as TcpClient;
        while (true)
        {
            string received = ReadTextMessage(client);
            Console.WriteLine("Received: {0}", received);
        }
        client.Close();
        Console.WriteLine("Connection closed");
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
}