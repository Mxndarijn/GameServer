using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using ClientSide.VR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Client
{
    private TcpClient _client;
    private NetworkStream _stream;
    private string _username;

    private byte[] _totalBuffer = Array.Empty<byte>();
    private readonly byte[] _buffer = new byte[1024];

    public Client()
    {
        OnMessage += async (_, json) => await ProcessMessageAsync(json);
        
        try
        {
            _client = new("84.26.134.162", 2460); // 84.26.134.162
            _stream = _client.GetStream();
            
            DataCommunication.SendData(_stream, "{\"id\": \" \"}");
            Console.WriteLine("Please enter your username: ");
            _username = Console.ReadLine();
            
            DataCommunication.SendData(_stream, (JsonFileReader.GetObjectAsString("Client\\Username", new Dictionary<string, string>()
                {
                    {"_name_", _username}
                })));
            _stream.BeginRead(_buffer, 0, 1024, OnRead, null);
            
            Console.WriteLine("Waiting for game to start");
            while (true)
            {
                
                Thread.Sleep(5);
            }
        }
        catch(Exception exception)
        {
            Console.WriteLine($"Could not connect with server... {exception}");
        }
    }

    private async Task ProcessMessageAsync(JObject json)
    {
        switch (json["id"].ToObject<string>())
        {
            case "game-created":
            {
                Console.WriteLine($"Game created: {json["data"]["name"]}");
                break;
            }
        }
    }

    public void WriteMessageToServer(string message)
    {
        var stream = new StreamWriter(_client.GetStream(), Encoding.ASCII);
        {
            stream.WriteLine(message);
            stream.Flush();
        }
    }

    private void OnRead(IAsyncResult readResult)
    {
        try
        {
            var numberOfBytes = _stream.EndRead(readResult);
            _totalBuffer = Concat(_totalBuffer, _buffer, numberOfBytes);
        }
        catch (Exception ex)
        {
            return;
        }

        while (_totalBuffer.Length >= 4)
        {
            var packetSize = BitConverter.ToInt32(_totalBuffer, 0);

            if (_totalBuffer.Length >= packetSize + 4)
            {
                var json = Encoding.UTF8.GetString(_totalBuffer, 4, packetSize);
                OnMessage?.Invoke(this, JObject.Parse(json));

                var newBuffer = new byte[_totalBuffer.Length - packetSize - 4];
                Array.Copy(_totalBuffer, packetSize + 4, newBuffer, 0, newBuffer.Length);
                _totalBuffer = newBuffer;
            }

            else
                break;
        }

        _stream.BeginRead(_buffer, 0, 1024, OnRead, null);
    }

    public event EventHandler<JObject> OnMessage;

    private static byte[] Concat(byte[] b1, byte[] b2, int count)
    {
        var r = new byte[b1.Length + count];
        Buffer.BlockCopy(b1, 0, r, 0, b1.Length);
        Buffer.BlockCopy(b2, 0, r, b1.Length, count);
        return r;
    }
}