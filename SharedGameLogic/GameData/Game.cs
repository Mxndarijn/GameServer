using System.Net.Sockets;
using System.Text;

namespace SharedGameLogic.GameData;

public class Game
{
    public Thread GameThread { get; set; }
    public Game(List<TcpClient> clients)
    {
        GameThread = new Thread(Run);
        GameThread.Start();
    }

    public void Run()
    {
        
    }

}